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

        public StockPricePublisherService(
            IStockPriceService stockPriceService,
            IEventPublisher eventPublisher,
            ISymbolValidator symbolValidator)
        {
            _stockPriceService = stockPriceService;
            _eventPublisher = eventPublisher;
            _symbolValidator = symbolValidator;
        }

        public async Task<Result> RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default)
        {

            if (!_symbolValidator.AllValid(prices.Select(p => p.Symbol)))
                return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid stock symbol.");

            if (prices.Any(p => p.Price <= 0))
                return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid Price");

            await _stockPriceService.RecordPricesAsync(prices);

            var priceChanges = prices.Select(p => new PriceChangedDto
            {
                Symbol = p.Symbol,
                NewPrice = p.Price
            }).ToList();

            var evt = new PriceBatchChangedEvent { Prices = priceChanges };
            await _eventPublisher.PublishAsync(evt, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default)
        {
            return await RecordAndPublishAsync(new[] { price }, cancellationToken);
        }
    }

}
