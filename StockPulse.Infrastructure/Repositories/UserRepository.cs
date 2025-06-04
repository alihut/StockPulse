using Microsoft.EntityFrameworkCore;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;
using StockPulse.Infrastructure.Data;

namespace StockPulse.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StockPulseDbContext _context;

        public UserRepository(StockPulseDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }

}
