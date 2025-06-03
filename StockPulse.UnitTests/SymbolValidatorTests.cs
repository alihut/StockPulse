using Microsoft.Extensions.Options;
using StockPulse.Application.Settings;
using StockPulse.Infrastructure.Services;

namespace StockPulse.UnitTests
{
    public class SymbolValidatorTests
    {
        private readonly SymbolValidator _validator;

        public SymbolValidatorTests()
        {
            var settings = Options.Create(new StockSettings
            {
                Symbols = new List<string> { "AAPL", "GOOGL", "MSFT", "TSLA" }
            });

            _validator = new SymbolValidator(settings);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_ForValidSymbol_IgnoringCase()
        {
            Assert.True(_validator.IsValid("aapl"));
            Assert.True(_validator.IsValid("GOOGL"));
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_ForInvalidSymbol()
        {
            Assert.False(_validator.IsValid("ABC"));
            Assert.False(_validator.IsValid("INVALID"));
        }

        [Fact]
        public void AllValid_ShouldReturnTrue_WhenAllSymbolsAreValid()
        {
            var symbols = new List<string> { "msft", "aapl", "GOOGL" };
            Assert.True(_validator.AllValid(symbols));
        }

        [Fact]
        public void AllValid_ShouldReturnFalse_WhenAnySymbolIsInvalid()
        {
            var symbols = new List<string> { "AAPL", "INVALID" };
            Assert.False(_validator.AllValid(symbols));
        }
    }
}