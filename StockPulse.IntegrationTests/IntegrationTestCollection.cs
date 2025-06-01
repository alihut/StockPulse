using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.IntegrationTests
{
    [CollectionDefinition("IntegrationTestCollection")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
    {
        // no code needed here
    }

}
