using StockPulse.Domain.Enums;

namespace StockPulse.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string username, UserRole role);
    }

}
