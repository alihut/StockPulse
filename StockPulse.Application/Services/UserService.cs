using StockPulse.Application.Helpers;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;

namespace StockPulse.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null) return null;

            var hashedInput = HashHelper.HashPassword(password);
            return user.PasswordHash == hashedInput ? user : null;
        }
    }
}
