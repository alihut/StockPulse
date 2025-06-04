using StockPulse.Messaging.Events;

namespace StockPulse.Application.Interfaces
{
    public interface IPriceBatchAlertEvaluator
    {
        Task EvaluateAsync(Guid batchId);
    }
}
