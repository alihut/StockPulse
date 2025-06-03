using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockPulse.Application.Settings;
using StockPulse.Domain.Enums;
using StockPulse.Infrastructure.Services;
using Xunit;

namespace StockPulse.UnitTests
{
    public class JwtTokenGeneratorTests
    {
        private readonly JwtTokenGenerator _generator;
        private readonly JwtSettings _settings;

        public JwtTokenGeneratorTests()
        {
            _settings = new JwtSettings
            {
                SecretKey = "1234567890superlongtestkeysecure!", // Must be at least 16 chars for HmacSha256
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationMinutes = 60
            };

            _generator = new JwtTokenGenerator(Options.Create(_settings));
        }

        [Fact]
        public void GenerateToken_ShouldReturn_ValidJwt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "testuser";
            var role = UserRole.Admin;

            // Act
            var token = _generator.GenerateToken(userId, username, role);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal(userId.ToString(), jwt.Subject);
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == username);
            Assert.Contains(jwt.Claims, c =>
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "admin");

            Assert.Equal(_settings.Issuer, jwt.Issuer);
            Assert.Equal(_settings.Audience, jwt.Audiences.First());
        }

        [Fact]
        public void GenerateToken_ShouldExpireInExpectedTime()
        {
            var token = _generator.GenerateToken(Guid.NewGuid(), "user", UserRole.EndUser);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var now = DateTime.UtcNow;
            var expiration = jwt.ValidTo;

            Assert.True(expiration > now);
            Assert.True(expiration <= now.AddMinutes(_settings.ExpirationMinutes + 1));
        }
    }
}
