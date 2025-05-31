using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;
using StockPulse.Infrastructure.Data;

namespace StockPulse.Infrastructure.Repositories;

public class StockPriceRepository : IStockPriceRepository
{
    private readonly StockPulseDbContext _context;

    public StockPriceRepository(StockPulseDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StockPrice price)
    {
        _context.StockPrices.Add(price);
        await _context.SaveChangesAsync();
    }
}