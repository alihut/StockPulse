
using StockPulse.Application.Helpers;

namespace StockPulse.UnitTests
{
    public class HashHelperTests
    {
        [Fact]
        public void HashPassword_ReturnsConsistentResult_ForSameInput()
        {
            // Arrange
            var password = "MySecurePassword";

            // Act
            var hash1 = HashHelper.HashPassword(password);
            var hash2 = HashHelper.HashPassword(password);

            // Assert
            Assert.Equal(hash1, hash2); // Deterministic
        }

        [Fact]
        public void HashPassword_ReturnsDifferentResult_ForDifferentInput()
        {
            var password1 = "Password1";
            var password2 = "Password2";

            var hash1 = HashHelper.HashPassword(password1);
            var hash2 = HashHelper.HashPassword(password2);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_DoesNotReturnOriginalPassword()
        {
            var password = "PlainText123";
            var hash = HashHelper.HashPassword(password);

            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void HashPassword_ReturnsNonEmptyHash()
        {
            var hash = HashHelper.HashPassword("anything");

            Assert.False(string.IsNullOrWhiteSpace(hash));
        }
    }
}
