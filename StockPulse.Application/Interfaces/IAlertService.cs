using StockPulse.Application.DTOs;
using StockPulse.Application.Models;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertService
    {
        Task<Result<Guid>> RegisterAlertAsync(CreateAlertRequestDto request);
        Task<Result<IEnumerable<AlertDto>>> GetUserAlertsAsync();
        Task<Result> DeleteAlertAsync(Guid id);
    }
}