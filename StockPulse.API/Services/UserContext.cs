using StockPulse.Application.Interfaces;
using System.Security.Claims;

namespace StockPulse.API.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");

            return userId;
        }
    }
}
