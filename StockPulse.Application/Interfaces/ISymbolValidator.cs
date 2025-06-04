namespace StockPulse.Application.Interfaces
{
    public interface ISymbolValidator
    {
        bool IsValid(string symbol);

        bool AllValid(IEnumerable<string> symbols);
    }

}
