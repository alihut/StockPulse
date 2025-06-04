using StockPulse.Application.DTOs;
using StockPulse.Application.Enums;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Models;
using StockPulse.Messaging.Events;

namespace StockPulse.Application.Services
{
    public class StockPricePublisherService : IStockPricePublisherService
    {
        private readonly IStockPriceService _stockPriceService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISymbolValidator _symbolValidator;
        private readonly ICacheService _cacheService;

        public StockPricePublisherService(
            IStockPriceService stockPriceService,
            IEventPublisher eventPublisher,
            ISymbolValidator symbolValidator,
            ICacheService cacheService)
        {
            _stockPriceService = stockPriceService;
            _eventPublisher = eventPublisher;
            _symbolValidator = symbolValidator;
            _cacheService = cacheService;
        }

        public async Task<Result> RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default)
        {

            if (!_symbolValidator.AllValid(prices.Select(p => p.Symbol)))
                return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid stock symbol.");

            if (prices.Any(p => p.Price <= 0))
                return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid Price");

            await _stockPriceService.RecordPricesAsync(prices);


            var batchId = Guid.NewGuid();
            _cacheService.Set($"PriceBatch:{batchId}", prices, TimeSpan.FromMinutes(1));
            var evt = new PriceBatchChangedEvent { BatchId = batchId };
            await _eventPublisher.PublishAsync(evt, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default)
        {
            return await RecordAndPublishAsync(new[] { price }, cancellationToken);
        }
    }

}
