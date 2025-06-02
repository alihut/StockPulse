using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertNotificationTests : IntegrationTestBase, IAsyncLifetime
    {
        public AlertNotificationTests(IntegrationTestFixture fixture) : base(fixture) { }

        public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();
        public async Task DisposeAsync() => await Factory.ResetDatabaseAsync();

        [Fact]
        public async Task User_Receives_Alert_Notification_Via_SignalR()
        {
            // Arrange
            var token = await LoginAsAsync(); // defaults to user1
            AuthenticateClient(token);

            var connection = new HubConnectionBuilder()
                .WithUrl($"{Client.BaseAddress}alerts", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler(); // for test server
                })
                .WithAutomaticReconnect()
                .Build();

            string? received = null;

            connection.On<object>("AlertTriggered", data =>
            {
                received = JsonSerializer.Serialize(data);
            });

            await connection.StartAsync();

            // Register alert
            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = "AAPL",
                PriceThreshold = 180,
                Type = 1 // AlertType.Below
            });

            registerResponse.EnsureSuccessStatusCode();

            // Trigger the alert by manually recording price < 180
            using var scope = Factory.Services.CreateScope();
            var stockService = scope.ServiceProvider.GetRequiredService<IStockPriceService>();
            await stockService.RecordPriceAsync(new RecordPriceRequestDto
            {
                Symbol = "AAPL",
                Price = 170
            });

            // Assert
            await Task.Delay(1000); // wait for SignalR message
            received.Should().NotBeNull();
            received.Should().Contain("AAPL");

            await connection.DisposeAsync();
        }

        [Fact]
        public async Task No_Notification_If_Threshold_Not_Met()
        {
            var token = await LoginAsAsync(); // defaults to user1
            AuthenticateClient(token);

            var connection = new HubConnectionBuilder()
                .WithUrl($"{Client.BaseAddress}alerts", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                })
                .WithAutomaticReconnect()
                .Build();

            string? received = null;
            connection.On<object>("AlertTriggered", data =>
            {
                received = JsonSerializer.Serialize(data);
            });

            await connection.StartAsync();

            var registerResponse = await Client.PostAsJsonAsync("/api/alert", new
            {
                Symbol = "GOOGL",
                PriceThreshold = 200,
                Type = 1 // Below
            });
            registerResponse.EnsureSuccessStatusCode();

            using var scope = Factory.Services.CreateScope();
            var stockService = scope.ServiceProvider.GetRequiredService<IStockPriceService>();
            await stockService.RecordPriceAsync(new RecordPriceRequestDto
            {
                Symbol = "TSLA",
                Price = 210
            });

            await Task.Delay(1000);
            received.Should().BeNull();

            await connection.DisposeAsync();
        }
    }

}
