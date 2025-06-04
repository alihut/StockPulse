using MassTransit;
using StockPulse.Application.Interfaces;
using StockPulse.Messaging.Events;

namespace StockPulse.Infrastructure.Messaging.Consumers
{
    public class PriceBatchChangedEventConsumer : IConsumer<PriceBatchChangedEvent>
    {
        private readonly IPriceBatchAlertEvaluator _priceBatchAlertEvaluator;

        public PriceBatchChangedEventConsumer(IPriceBatchAlertEvaluator priceBatchAlertEvaluator)
        {
            _priceBatchAlertEvaluator = priceBatchAlertEvaluator;
        }

        public async Task Consume(ConsumeContext<PriceBatchChangedEvent> context)
        {
            await _priceBatchAlertEvaluator.EvaluateAsync(context.Message.BatchId);
        }
    }
}
