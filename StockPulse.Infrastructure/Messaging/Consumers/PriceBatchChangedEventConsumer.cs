using MassTransit;
using StockPulse.Application.Interfaces;
using StockPulse.Messaging.Events;

namespace StockPulse.Infrastructure.Messaging.Consumers
{
    public class PriceBatchChangedEventConsumer : IConsumer<PriceBatchChangedEvent>
    {
        private readonly IAlertEvaluationService _alertEvaluationService;

        public PriceBatchChangedEventConsumer(IAlertEvaluationService alertEvaluationService)
        {
            _alertEvaluationService = alertEvaluationService;
        }

        public async Task Consume(ConsumeContext<PriceBatchChangedEvent> context)
        {
            await _alertEvaluationService.EvaluateAlertsAsync(context.Message.Prices);
        }
    }
}
