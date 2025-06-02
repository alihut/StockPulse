using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StockPulse.API.Hubs
{
    [Authorize]
    public class AlertHub : Hub
    {
        // Called when a client connects
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"User {userId} connected to AlertHub.");
            await base.OnConnectedAsync();
        }

        // Called when a client disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"User {userId} disconnected from AlertHub.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
