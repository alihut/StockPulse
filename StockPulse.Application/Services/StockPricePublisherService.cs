using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Messaging.Events;

namespace StockPulse.Application.Services
{
    public class StockPricePublisherService : IStockPricePublisherService
    {
        private readonly IStockPriceService _stockPriceService;
        private readonly IEventPublisher _eventPublisher;

        public StockPricePublisherService(
            IStockPriceService stockPriceService,
            IEventPublisher eventPublisher)
        {
            _stockPriceService = stockPriceService;
            _eventPublisher = eventPublisher;
        }

        public async Task RecordAndPublishAsync(IEnumerable<RecordPriceRequestDto> prices, CancellationToken cancellationToken = default)
        {
            await _stockPriceService.RecordPricesAsync(prices);

            var priceChanges = prices.Select(p => new PriceChangedDto
            {
                Symbol = p.Symbol,
                NewPrice = p.Price
            }).ToList();

            var evt = new PriceBatchChangedEvent { Prices = priceChanges };
            await _eventPublisher.PublishAsync(evt, cancellationToken);
        }

        public async Task RecordAndPublishAsync(RecordPriceRequestDto price, CancellationToken cancellationToken = default)
        {
            await RecordAndPublishAsync(new[] { price }, cancellationToken);
        }
    }

}
