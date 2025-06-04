namespace StockPulse.Application.DTOs
{
    public class RecordPriceRequestDto
    {
        public string Symbol { get; set; }

        public decimal Price { get; set; }
    }
}
