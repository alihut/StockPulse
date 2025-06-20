﻿using RedLockNet;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Enums;

namespace StockPulse.Application.Services
{
    public class PriceBatchAlertEvaluator : IPriceBatchAlertEvaluator
    {
        private readonly IAlertRepository _alertRepo;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;
        private readonly IDistributedLockFactory _lockFactory;

        public PriceBatchAlertEvaluator(
            IAlertRepository alertRepo,
            INotificationService notificationService,
            ICacheService cacheService,
            IDistributedLockFactory lockFactory)
        {
            _alertRepo = alertRepo;
            _notificationService = notificationService;
            _cacheService = cacheService;
            _lockFactory = lockFactory;
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
                    // 🔐 Acquire distributed lock per alert
                    var resource = $"alert-lock:{alert.Id}";
                    using var redLock = await _lockFactory.CreateLockAsync(resource, TimeSpan.FromSeconds(10));

                    if (!redLock.IsAcquired)
                    {
                        // Another instance is processing this alert, skip
                        continue;
                    }

                    bool shouldTrigger =
                        (alert.Type == AlertType.Above && priceChange.Price > alert.PriceThreshold) ||
                        (alert.Type == AlertType.Below && priceChange.Price < alert.PriceThreshold);

                    if (shouldTrigger)
                    {
                        alert.IsActive = false;
                        await _alertRepo.UpdateAsync(alert);

                        await _notificationService.NotifyUserAsync(alert.UserId, new
                        {
                            Symbol = alert.Symbol,
                            Price = priceChange.Price,
                            TriggeredAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }
    }
}
