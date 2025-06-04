using System.IdentityModel.Tokens.Jwt;

namespace StockPulse.IntegrationTests.Helpers
{
    public static class JwtHelper
    {
        public static Guid GetUserIdFromToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            return Guid.Parse(userIdClaim?.Value ?? throw new InvalidOperationException("User ID claim not found"));
        }

    }
}
