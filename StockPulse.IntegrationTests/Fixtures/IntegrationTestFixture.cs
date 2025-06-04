using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockPulse.Infrastructure.Data;

namespace StockPulse.IntegrationTests.Fixtures
{
    public class IntegrationTestFixture : WebApplicationFactory<API.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            Environment.SetEnvironmentVariable("DISABLE_SIMULATOR", "true");

            builder.ConfigureServices(services =>
            {
                // Replace services here (e.g., use in-memory DB, mocks, etc.)
            });
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StockPulseDbContext>();

            await db.Alerts.ExecuteDeleteAsync();
            await db.StockPrices.ExecuteDeleteAsync();
        }
    }

}
