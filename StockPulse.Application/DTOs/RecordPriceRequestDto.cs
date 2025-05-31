using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Application.DTOs
{
    public class RecordPriceRequestDto
    {
        public string Symbol { get; set; }

        public decimal Price { get; set; }
    }
}
