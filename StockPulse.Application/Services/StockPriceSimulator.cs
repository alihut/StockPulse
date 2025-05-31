using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Settings;

namespace StockPulse.Application.Services
{
    public class StockPriceSimulator : BackgroundService
    {
        private readonly List<string> _symbols;
        private readonly Random _random = new();
        private readonly IStockPriceService _stockPriceService;

        public StockPriceSimulator(
            IStockPriceService stockPriceService, 
            IOptions<StockSettings> stockSettingsOptions)
        {
            _stockPriceService = stockPriceService;
            _symbols = stockSettingsOptions.Value.Symbols;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var symbol in _symbols)
                {
                    var basePrice = 100 + _random.Next(50);
                    var changePercent = (decimal)(_random.NextDouble() * 0.05 - 0.025); // ±2.5%
                    var newPrice = basePrice + basePrice * changePercent;
                    var rounded = Math.Round(newPrice, 2);

                    Console.WriteLine($"[Simulator] Symbol: {symbol} | Price: {rounded} | Time: {DateTime.UtcNow}");

                    await _stockPriceService.RecordPriceAsync(new RecordPriceRequestDto
                    {
                        Symbol = symbol,
                        Price = rounded
                    });
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

        }
    }
}