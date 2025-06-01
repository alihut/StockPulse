using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;
using StockPulse.Domain.Enums;

namespace StockPulse.Application.Interfaces
{
    public interface IAlertRepository
    {
        Task<bool> ExistsAsync(Guid userId, string symbol, decimal threshold, AlertType type);
        Task AddAlertAsync(Alert alert);
        Task<IEnumerable<Alert>> GetActiveAlertsAsync(Guid userId);

        Task<IEnumerable<Alert>> GetActiveAlertsBySymbolAsync(string symbol);
        Task UpdateAsync(Alert alert);

        Task DeleteAlertAsync(Guid alertId);
    }
}
