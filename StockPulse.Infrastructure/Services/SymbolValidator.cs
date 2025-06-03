using Microsoft.Extensions.Options;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Settings;

namespace StockPulse.Infrastructure.Services;

public class SymbolValidator : ISymbolValidator
{
    private readonly HashSet<string> _validSymbols;

    public SymbolValidator(IOptions<StockSettings> settings)
    {
        _validSymbols = settings.Value.Symbols
            .Select(s => s.ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsValid(string symbol)
    {
        return _validSymbols.Contains(symbol.ToUpperInvariant());
    }

    public bool AllValid(IEnumerable<string> symbols)
    {
        return symbols.All(symbol => IsValid(symbol));
    }
}