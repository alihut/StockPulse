using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Enums;
using StockPulse.IntegrationTests.Helpers;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertNotificationTests : IntegrationTestBase, IAsyncLifetime
    {
        public AlertNotificationTests(IntegrationTestFixture fixture) : base(fixture) { }

        public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();
        public async Task DisposeAsync() => await Factory.ResetDatabaseAsync();

        [Theory]
        [InlineData("AAPL", 180, 170, 1)]
        [InlineData("MSFT", 290, 300, 0)]
        public async Task User_Receives_Alert_Notification_Via_SignalR(string symbol, decimal threshold, decimal triggeredPrice, int alertType)
        {
            // Arrange
            var token = await LoginAsAsync();
            AuthenticateClient(token);

            var connection = CreateAlertHubConnectionAsync(token);

            string? received = null;

            connection.On<object>("AlertTriggered", data =>
            {
                received = JsonSerializer.Serialize(data);
            });

            await connection.StartAsync();

            // Register alert
            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = symbol,
                PriceThreshold = threshold,
                Type = alertType 
            });

            registerResponse.EnsureSuccessStatusCode();

            // Trigger the alert
            using var scope = Factory.Services.CreateScope();
            var stockPricePublisherService = scope.ServiceProvider.GetRequiredService<IStockPricePublisherService>();
            await stockPricePublisherService.RecordAndPublishAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = triggeredPrice
            });

            // Assert
            await Task.Delay(1000);
            received.Should().NotBeNull();
            received.Should().Contain(symbol);

            await connection.DisposeAsync();
        }

        [Theory]
        [InlineData("AAPL", 180, 185, 1)] // Below threshold not met
        [InlineData("MSFT", 300, 290, 0)] // Above threshold not met
        public async Task No_Notification_If_Threshold_Not_Met(string symbol, decimal threshold, decimal actualPrice, int alertType)
        {
            // Arrange
            var token = await LoginAsAsync();
            AuthenticateClient(token);

            var connection = CreateAlertHubConnectionAsync(token);

            string? received = null;

            connection.On<object>("AlertTriggered", data =>
            {
                received = JsonSerializer.Serialize(data);
            });

            await connection.StartAsync();

            // Register alert
            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = symbol,
                PriceThreshold = threshold,
                Type = alertType
            });

            registerResponse.EnsureSuccessStatusCode();

            // Trigger a price that does NOT satisfy the alert condition
            using var scope = Factory.Services.CreateScope();
            var stockPricePublisherService = scope.ServiceProvider.GetRequiredService<IStockPricePublisherService>();
            await stockPricePublisherService.RecordAndPublishAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = actualPrice
            });

            // Assert
            await Task.Delay(1000);
            received.Should().BeNull();

            await connection.DisposeAsync();
        }

        [Theory]
        [InlineData("AAPL", 180, 170, 1)] // Below threshold
        public async Task Connection_Reconnects_And_Still_Receives_Alerts(string symbol, decimal threshold, decimal triggeredPrice, int alertType)
        {
            // Arrange
            var token = await LoginAsAsync();
            AuthenticateClient(token);

            var connection = CreateAlertHubConnectionAsync(token);

            string? received = null;
            connection.On<object>("AlertTriggered", data =>
            {
                received = JsonSerializer.Serialize(data);
            });

            await connection.StartAsync();
            connection.State.Should().Be(HubConnectionState.Connected);

            // Register alert
            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = symbol,
                PriceThreshold = threshold,
                Type = alertType
            });
            registerResponse.EnsureSuccessStatusCode();

            // Simulate a dropped connection
            await connection.StopAsync();
            connection.State.Should().Be(HubConnectionState.Disconnected);

            // Reconnect
            await connection.StartAsync();
            connection.State.Should().Be(HubConnectionState.Connected);

            // Trigger the alert
            using var scope = Factory.Services.CreateScope();
            var stockPricePublisherService = scope.ServiceProvider.GetRequiredService<IStockPricePublisherService>();
            await stockPricePublisherService.RecordAndPublishAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = triggeredPrice
            });

            // Assert
            await Task.Delay(1000);
            received.Should().NotBeNull();
            received.Should().Contain(symbol);

            await connection.DisposeAsync();
        }

        [Theory]
        [InlineData("AAPL", 180, 170, 1)] // Below threshold
        [InlineData("MSFT", 290, 300, 0)] // Above threshold
        public async Task Alert_Is_Deactivated_After_Triggered(string symbol, decimal threshold, decimal triggeredPrice, int alertType)
        {
            // Arrange
            var token = await LoginAsAsync();
            AuthenticateClient(token);

            var connection = CreateAlertHubConnectionAsync(token);

            await connection.StartAsync();

            // Register alert
            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = symbol,
                PriceThreshold = threshold,
                Type = alertType
            });

            registerResponse.EnsureSuccessStatusCode();

            // Trigger the alert
            using var scope = Factory.Services.CreateScope();
            var stockPricePublisherService = scope.ServiceProvider.GetRequiredService<IStockPricePublisherService>();
            var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertRepository>();

            await stockPricePublisherService.RecordAndPublishAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = triggeredPrice
            });

            // Assert: wait briefly for processing
            await Task.Delay(1000);

            var userId = JwtHelper.GetUserIdFromToken(token);
            var alertStillExists = await alertRepo.ExistsAsync(userId, symbol, (AlertType)alertType);
            alertStillExists.Should().BeFalse();

            await connection.DisposeAsync();
        }


        [Theory]
        [InlineData("user1", "AAPL", 150, 155, 0)] // Only user1 should receive notification
        [InlineData("user2", "TSLA", 700, 710, 0)] // Only user2 should receive notification
        public async Task Multiple_Users_Receive_Only_Their_Alerts(string receivingUser, string symbol, decimal threshold, decimal triggeredPrice, int alertType)
        {
            // Arrange: Login both users
            var token1 = await LoginAsAsync("user1");
            var token2 = await LoginAsAsync("user2");

            var connection1 = CreateAlertHubConnectionAsync(token1);
            string? received1 = null;
            connection1.On<object>("AlertTriggered", data =>
            {
                received1 = JsonSerializer.Serialize(data);
            });
            await connection1.StartAsync();

            var connection2 = CreateAlertHubConnectionAsync(token2);
            string? received2 = null;
            connection2.On<object>("AlertTriggered", data =>
            {
                received2 = JsonSerializer.Serialize(data);
            });
            await connection2.StartAsync();

            Client.DefaultRequestHeaders.Authorization = null;
            // Register alerts
            if (receivingUser == "user1")
            {
                AuthenticateClient(token1);
            }
            else
            {
                AuthenticateClient(token2);
            }

            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = symbol,
                PriceThreshold = threshold,
                Type = alertType
            });

            registerResponse.EnsureSuccessStatusCode();

            // Trigger the alert
            using var scope = Factory.Services.CreateScope();
            var stockPricePublisherService = scope.ServiceProvider.GetRequiredService<IStockPricePublisherService>();
            await stockPricePublisherService.RecordAndPublishAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = triggeredPrice
            });

            // Assert
            await Task.Delay(1000);

            if (receivingUser == "user1")
            {
                received1.Should().NotBeNull();
                received2.Should().BeNull();
            }
            else
            {
                received2.Should().NotBeNull();
                received1.Should().BeNull();
            }

            await connection1.DisposeAsync();
            await connection2.DisposeAsync();
        }


    }

}
