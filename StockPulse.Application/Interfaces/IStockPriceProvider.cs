namespace StockPulse.Application.Interfaces
{
    public interface IStockPriceProvider
    {
        IReadOnlyDictionary<string, decimal> GetCurrentPrices();
    }
}
