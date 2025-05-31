using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using StockPulse.Application.Interfaces;

namespace StockPulse.Infrastructure.Services
{
    public class StockPriceSimulator : BackgroundService, IStockPriceProvider
    {
        private readonly ConcurrentDictionary<string, decimal> _prices = new();
        private readonly string[] _symbols = new[] { "AAPL", "GOOGL", "MSFT" };
        private readonly Random _random = new();

        public IReadOnlyDictionary<string, decimal> GetCurrentPrices() => _prices;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initialize baseline prices
            foreach (var symbol in _symbols)
                _prices[symbol] = 100 + _random.Next(50); // e.g., 100–150 baseline

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var symbol in _symbols)
                {
                    var current = _prices[symbol];
                    var changePercent = (decimal)(_random.NextDouble() * 0.05 - 0.025); // ±2.5%
                    var newPrice = current + current * changePercent;
                    _prices[symbol] = Math.Round(newPrice, 2);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
