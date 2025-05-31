using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertService
    {
        Task RegisterAlertAsync(CreateAlertRequestDto request);
        Task<IEnumerable<AlertDto>> GetUserAlertsAsync(Guid userId);
        Task DeleteAlertAsync(Guid id);
    }
}