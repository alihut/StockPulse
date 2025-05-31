using StockPulse.Application.DTOs;

namespace StockPulse.Application.Interfaces
{
    public interface IStockPriceService
    {
        Task RecordPriceAsync(RecordPriceRequestDto request);
    }

}
