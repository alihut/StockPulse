namespace StockPulse.Domain.Entities
{
    public class StockPrice : BaseEntity
    {
        public string Symbol { get; set; } = null!;
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
