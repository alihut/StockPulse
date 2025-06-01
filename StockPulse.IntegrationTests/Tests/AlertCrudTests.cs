using StockPulse.IntegrationTests.Base;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertCrudTests : IntegrationTestBase
    {
        public AlertCrudTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Returns_Conflict_When_Alert_Already_Exists()
        {
            var token = await AuthHelper.LoginAsync(Client, "user1", "Password123");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var alertRequest = new
            {
                Symbol = "AAPL",
                PriceThreshold = 100,
                Type = 0
            };

            await Client.PostAsJsonAsync("/api/alert", alertRequest);
            var response = await Client.PostAsJsonAsync("/api/alert", alertRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task User_registers_alert_successfully()
        {
            // Arrange
            var token = await AuthHelper.LoginAsync(Client, "user1", "Password123");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var alertRequest = new
            {
                Symbol = "GOOGL",
                PriceThreshold = 180,
                Type = 0
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/alert", alertRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
