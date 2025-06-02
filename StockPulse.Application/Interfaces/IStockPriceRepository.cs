using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPriceRepository
    {
        Task AddAsync(StockPrice price);
        Task AddAsync(IEnumerable<StockPrice> prices);
    }
}
