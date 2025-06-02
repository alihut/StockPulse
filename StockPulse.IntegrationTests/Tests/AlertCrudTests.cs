using StockPulse.IntegrationTests.Base;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;

namespace StockPulse.IntegrationTests.Tests
{
    public class AlertCrudTests : IntegrationTestBase, IAsyncLifetime
    {
        public AlertCrudTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();
        public async Task DisposeAsync() => await Factory.ResetDatabaseAsync();

        [Fact]
        public async Task Returns_Conflict_When_Alert_Already_Exists()
        {
            // Arrange
            var token = await LoginAsAsync(); // defaults to user1
            AuthenticateClient(token);

            var alertRequest = new
            {
                Symbol = "AAPL",
                PriceThreshold = 100,
                Type = 0 // AlertType.Above
            };

            // First alert registration should succeed
            var firstResponse = await Client.PostAsJsonAsync("/api/alert", alertRequest);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act - Try to register the same alert again
            var secondResponse = await Client.PostAsJsonAsync("/api/alert", alertRequest);

            // Assert
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }


        [Theory]
        [InlineData("AAPL", 180, 1)] // Below
        [InlineData("MSFT", 300, 0)] // Above
        public async Task User_Registers_Alert_Successfully(string symbol, decimal threshold, int type)
        {
            var token = await LoginAsAsync(); // defaults to user1
            AuthenticateClient(token);

            var alertRequest = new { Symbol = symbol, PriceThreshold = threshold, Type = type };

            var response = await Client.PostAsJsonAsync("/api/alert", alertRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
