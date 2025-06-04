using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Services
{
    public class StockPriceService : IStockPriceService
    {
        private readonly IStockPriceRepository _repository;
        private readonly IMapper _mapper;

        public StockPriceService(IStockPriceRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task RecordPriceAsync(RecordPriceRequestDto request)
        {
            var stockPrice = _mapper.Map<StockPrice>(request);
            await _repository.AddAsync(stockPrice);
        }

        public async Task RecordPricesAsync(IEnumerable<RecordPriceRequestDto> prices)
        {
            var stockPrices = _mapper.Map<IEnumerable<StockPrice>>(prices);
            await _repository.AddAsync(stockPrices);
        }
    }
}
