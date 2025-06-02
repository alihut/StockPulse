using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Settings;
using StockPulse.Messaging.Events;

namespace StockPulse.Application.Services
{
    public class StockPriceSimulator : BackgroundService
    {
        private readonly List<string> _symbols;
        private readonly Random _random = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStockPricePublisherService _stockPricePublisherService;
        private readonly ILogger<StockPriceSimulator> _logger;

        public StockPriceSimulator(
            IServiceProvider serviceProvider,
            IOptions<StockSettings> stockSettingsOptions,
            IEventPublisher eventPublisher,
            IStockPricePublisherService stockPricePublisherService,
            ILogger<StockPriceSimulator> logger)
        {
            _serviceProvider = serviceProvider;
            _eventPublisher = eventPublisher;
            _stockPricePublisherService = stockPricePublisherService;
            _logger = logger;
            _symbols = stockSettingsOptions.Value.Symbols;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var recordList = GeneratePriceRecords();

                await _stockPricePublisherService.RecordAndPublishAsync(recordList);

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

        }

        private List<RecordPriceRequestDto> GeneratePriceRecords()
        {
            var recordList = new List<RecordPriceRequestDto>();

            foreach (var symbol in _symbols)
            {
                var basePrice = 100 + _random.Next(50);
                var changePercent = (decimal)(_random.NextDouble() * 0.05 - 0.025);
                var newPrice = basePrice + basePrice * changePercent;
                var rounded = Math.Round(newPrice, 2);

                _logger.LogInformation($"[Simulator] Symbol: {symbol} | Price: {rounded} | Time: {DateTime.UtcNow}");

                recordList.Add(new RecordPriceRequestDto
                {
                    Symbol = symbol,
                    Price = rounded
                });
            }

            return recordList;
        }
    }
}