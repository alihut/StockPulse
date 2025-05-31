using StockPulse.Application.DTOs;
using StockPulse.Application.Models;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertService
    {
        Task<Result<Guid>> RegisterAlertAsync(CreateAlertRequestDto request);
        Task<Result<IEnumerable<AlertDto>>> GetUserAlertsAsync();
        Task<Result> DeleteAlertAsync(Guid id);
    }
}