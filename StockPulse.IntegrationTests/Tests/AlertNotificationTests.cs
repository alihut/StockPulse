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
            var stockService = scope.ServiceProvider.GetRequiredService<IStockPriceService>();
            await stockService.RecordPriceAsync(new RecordPriceRequestDto
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
            var stockService = scope.ServiceProvider.GetRequiredService<IStockPriceService>();
            await stockService.RecordPriceAsync(new RecordPriceRequestDto
            {
                Symbol = symbol,
                Price = actualPrice
            });

            // Assert
            await Task.Delay(1000);
            received.Should().BeNull();

            await connection.DisposeAsync();
        }

    }

}
