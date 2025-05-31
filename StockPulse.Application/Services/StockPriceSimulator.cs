using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;

namespace StockPulse.Application.Services
{
    public class StockPriceSimulator : BackgroundService
    {
        private readonly ConcurrentDictionary<string, decimal> _prices = new();
        private readonly string[] _symbols = new[] { "AAPL", "GOOGL", "MSFT" };
        private readonly Random _random = new();
        private readonly IStockPriceService _stockPriceService;

        public StockPriceSimulator(IStockPriceService stockPriceService)
        {
            _stockPriceService = stockPriceService;
        }

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
                    var rounded = Math.Round(newPrice, 2);
                    _prices[symbol] = rounded;

                    var recordRequest = new RecordPriceRequestDto()
                    {
                        Symbol = symbol,
                        Price = rounded
                    };
                    // Save to database using service
                    await _stockPriceService.RecordPriceAsync(recordRequest);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}