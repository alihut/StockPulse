using StockPulse.Application.DTOs;
using StockPulse.Application.Models;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPricePublisherService
    {
        Task<Result> RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default);
        Task<Result> RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default);
    }
}
