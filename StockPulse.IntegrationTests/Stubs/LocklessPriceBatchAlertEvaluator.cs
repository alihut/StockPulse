using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Enums;

namespace StockPulse.IntegrationTests.Stubs
{
    public class LocklessPriceBatchAlertEvaluator : IPriceBatchAlertEvaluator
    {
        private readonly IAlertRepository _alertRepo;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public LocklessPriceBatchAlertEvaluator(
            IAlertRepository alertRepo,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _alertRepo = alertRepo;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }


        public async Task EvaluateAsync(Guid batchId)
        {
            var prices = _cacheService.Get<IEnumerable<RecordPriceRequestDto>>($"PriceBatch:{batchId}");
            if (prices == null) return;

            var symbols = prices.Select(p => p.Symbol).Distinct();
            var alerts = await _alertRepo.GetActiveAlertsBySymbolsAsync(symbols);

            foreach (var priceChange in prices)
            {
                var matchingAlerts = alerts.Where(a => a.Symbol == priceChange.Symbol).ToList();

                foreach (var alert in matchingAlerts)
                {
                    // ❌ NO LOCK HERE

                    bool shouldTrigger =
                        alert.Type == AlertType.Above && priceChange.Price > alert.PriceThreshold ||
                        alert.Type == AlertType.Below && priceChange.Price < alert.PriceThreshold;

                    if (shouldTrigger)
                    {
                        alert.IsActive = false;
                        await _alertRepo.UpdateAsync(alert);

                        await _notificationService.NotifyUserAsync(alert.UserId, new
                        {
                            alert.Symbol,
                            priceChange.Price,
                            TriggeredAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }
    }

}
