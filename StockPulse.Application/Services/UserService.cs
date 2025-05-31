using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPulse.Application.Helpers;

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

        //private string ComputeSha256Hash(string rawData)
        //{
        //    using var sha256 = SHA256.Create();
        //    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        //    return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        //}
    }
}
