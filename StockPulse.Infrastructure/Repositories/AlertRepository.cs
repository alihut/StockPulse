using Microsoft.EntityFrameworkCore;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;
using StockPulse.Infrastructure.Data;

public class AlertRepository : IAlertRepository
{
    private readonly StockPulseDbContext _context;

    public AlertRepository(StockPulseDbContext context)
    {
        _context = context;
    }

    public async Task AddAlertAsync(Alert alert)
    {
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync(Guid userId)
    {
        return await _context.Alerts
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync();
    }

    public async Task DeleteAlertAsync(Guid alertId)
    {
        var alert = await _context.Alerts.FindAsync(alertId);
        if (alert != null)
        {
            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
        }
    }
}