using Microsoft.AspNetCore.SignalR;
using StockPulse.API.Hubs;
using StockPulse.Application.Interfaces;

namespace StockPulse.API.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<AlertHub> _hubContext;

        public SignalRNotificationService(IHubContext<AlertHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyUserAsync(Guid userId, object payload)
        {
            return _hubContext.Clients.User(userId.ToString())
                .SendAsync("AlertTriggered", payload);
        }
    }

}
