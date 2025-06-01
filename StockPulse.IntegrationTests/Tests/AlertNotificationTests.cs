using FluentAssertions;
using StockPulse.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Base;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertNotificationTests : IntegrationTestBase
    {
        public AlertNotificationTests(IntegrationTestFixture fixture) : base(fixture) { }

        //[Fact]
        //public async Task User_receives_alert_notification_via_signalr()
        //{
        //    var token = await AuthHelper.LoginAsync(Client, "user1", "Password123");

        //    var connection = SignalRHelper.CreateConnection(token, $"{Client.BaseAddress}alertHub");
        //    string? received = null;

        //    connection.On<object>("AlertTriggered", data =>
        //    {
        //        received = JsonSerializer.Serialize(data);
        //    });

        //    await connection.StartAsync();

        //    await Client.PostAsJsonAsync("/api/alert", new
        //    {
        //        userId = "11111111-1111-1111-1111-111111111111",
        //        symbol = "AAPL",
        //        priceThreshold = 180,
        //        type = "Below"
        //    });

        //    await YourHelper.TriggerAlertFor("AAPL", 179.99m, Factory.Services);

        //    await Task.Delay(1000);
        //    received.Should().Contain("AAPL");
        //}
    }

}
