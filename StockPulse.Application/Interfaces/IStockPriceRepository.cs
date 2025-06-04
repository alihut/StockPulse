using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPriceRepository
    {
        Task AddAsync(StockPrice price);
        Task AddAsync(IEnumerable<StockPrice> prices);
    }
}
