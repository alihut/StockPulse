using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.IntegrationTests
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
