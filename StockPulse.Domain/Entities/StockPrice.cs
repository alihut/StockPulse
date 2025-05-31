using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Domain.Entities
{
    public class StockPrice : BaseEntity
    {
        public string Symbol { get; set; } = null!;
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
