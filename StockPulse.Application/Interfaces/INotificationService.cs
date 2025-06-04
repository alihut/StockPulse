namespace StockPulse.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyUserAsync(Guid userId, object payload);
    }
}
