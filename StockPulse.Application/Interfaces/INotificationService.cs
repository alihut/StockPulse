using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPulse.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyUserAsync(Guid userId, object payload);
    }
}
