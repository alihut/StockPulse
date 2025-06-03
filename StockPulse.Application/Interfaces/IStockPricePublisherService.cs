using StockPulse.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPulse.Application.Models;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPricePublisherService
    {
        Task<Result> RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default);
        Task<Result> RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default);
    }
}
