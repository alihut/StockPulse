using StockPulse.Application.Interfaces;
using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Services
{
    public class StockPriceService : IStockPriceService
    {
        private readonly IStockPriceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAlertEvaluationService _alertEvaluationService;

        public StockPriceService(IStockPriceRepository repository, 
            IMapper mapper,
            IAlertEvaluationService alertEvaluationService)
        {
            _repository = repository;
            _mapper = mapper;
            _alertEvaluationService = alertEvaluationService;
        }
        public async Task RecordPriceAsync(RecordPriceRequestDto request)
        {
            var stockPrice = _mapper.Map<StockPrice>(request);
            await _repository.AddAsync(stockPrice);
            await _alertEvaluationService.EvaluateAlertsAsync(request.Symbol, request.Price);
        }
    }
}
