using StockPulse.Messaging.Events;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertEvaluationService
    {
        Task EvaluateAlertsAsync(List<PriceChangedDto> prices);
    }
}
