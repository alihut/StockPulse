using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertRepository
    {
        Task AddAlertAsync(Alert alert);
        Task<IEnumerable<Alert>> GetActiveAlertsAsync(Guid userId);
        Task DeleteAlertAsync(Guid alertId);
    }
}
