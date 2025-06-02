using StockPulse.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPricePublisherService
    {
        Task RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default);
        Task RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default);
    }
}
