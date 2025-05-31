namespace StockPulse.Application.Interfaces
{
    public interface IAlertEvaluationService
    {
        Task EvaluateAlertsAsync(string symbol, decimal newPrice);
    }
}
