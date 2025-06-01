using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Base;
using StockPulse.IntegrationTests.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertNotificationTests : IntegrationTestBase
    {
        public AlertNotificationTests(IntegrationTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task User_Receives_Alert_Notification_Via_SignalR()
        {
            // Arrange
            var token = await AuthHelper.LoginAsync(Client, "user1", "Password123");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var connection = new HubConnectionBuilder()
                .WithUrl($"{Client.BaseAddress}alerts", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler(); // for test server
                })
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                })
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

    }

}
