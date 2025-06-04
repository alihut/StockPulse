using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
    }

}
