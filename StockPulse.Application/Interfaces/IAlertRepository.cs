using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertRepository
    {
        Task AddAlertAsync(Alert alert);
        Task<IEnumerable<Alert>> GetActiveAlertsAsync(Guid userId);

        Task<IEnumerable<Alert>> GetActiveAlertsBySymbolAsync(string symbol);
        Task UpdateAsync(Alert alert);

        Task DeleteAlertAsync(Guid alertId);
    }
}
