using StockPulse.Domain.Entities;

namespace StockPulse.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
