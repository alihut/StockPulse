using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Domain
{
    public class StockSymbol
    {
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
