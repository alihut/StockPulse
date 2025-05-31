using StockPulse.Application.Interfaces;
using StockPulse.Domain.Enums;

namespace StockPulse.Application.Services
{
    public class AlertEvaluationService : IAlertEvaluationService
    {
        private readonly IAlertRepository _alertRepo;
        private readonly INotificationService _notificationService;

        public AlertEvaluationService(IAlertRepository alertRepo, INotificationService notificationService)
        {
            _alertRepo = alertRepo;
            _notificationService = notificationService;
        }

        public async Task EvaluateAlertsAsync(string symbol, decimal newPrice)
        {
            var alerts = await _alertRepo.GetActiveAlertsBySymbolAsync(symbol);

            foreach (var alert in alerts)
            {
                bool shouldTrigger =
                    (alert.Type == AlertType.Above && newPrice > alert.PriceThreshold) ||
                    (alert.Type == AlertType.Below && newPrice < alert.PriceThreshold);

                if (shouldTrigger)
                {
                    alert.IsActive = false;
                    await _alertRepo.UpdateAsync(alert);

                    await _notificationService.NotifyUserAsync(alert.UserId, new
                    {
                        Symbol = alert.Symbol,
                        Price = newPrice,
                        TriggeredAt = DateTime.UtcNow
                    });
                }
            }
        }
    }

}
