using StockPulse.IntegrationTests.Fixtures;

namespace StockPulse.IntegrationTests.Base
{
    [Collection("IntegrationTestCollection")]
    public abstract class IntegrationTestBase
    {
        protected readonly IntegrationTestFixture Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(IntegrationTestFixture fixture)
        {
            Factory = fixture;
            Client = Factory.CreateClient();
        }
    }

}
