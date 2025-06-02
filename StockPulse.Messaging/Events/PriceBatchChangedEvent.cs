namespace StockPulse.Messaging.Events
{
    public class PriceBatchChangedEvent
    {
        public List<PriceChangedDto> Prices { get; set; } = new();
    }

    public class PriceChangedDto
    {
        public string Symbol { get; set; } = default!;
        public decimal NewPrice { get; set; }
    }
}
