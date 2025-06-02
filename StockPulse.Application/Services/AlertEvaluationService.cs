using StockPulse.Application.Interfaces;
using StockPulse.Domain.Enums;
using StockPulse.Messaging.Events;

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

        public async Task EvaluateAlertsAsync(List<PriceChangedDto> prices)
        {
            var symbols = prices.Select(p => p.Symbol).Distinct();
            var alerts = await _alertRepo.GetActiveAlertsBySymbolsAsync(symbols);

            foreach (var priceChange in prices)
            {
                var matchingAlerts = alerts.Where(a => a.Symbol == priceChange.Symbol).ToList();

                foreach (var alert in matchingAlerts)
                {
                    bool shouldTrigger =
                        (alert.Type == AlertType.Above && priceChange.NewPrice > alert.PriceThreshold) ||
                        (alert.Type == AlertType.Below && priceChange.NewPrice < alert.PriceThreshold);

                    if (shouldTrigger)
                    {
                        alert.IsActive = false;
                        await _alertRepo.UpdateAsync(alert);

                        await _notificationService.NotifyUserAsync(alert.UserId, new
                        {
                            Symbol = alert.Symbol,
                            Price = priceChange.NewPrice,
                            TriggeredAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }

    }

}
