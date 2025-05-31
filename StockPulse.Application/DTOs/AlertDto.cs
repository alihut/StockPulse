using StockPulse.Domain.Enums;

namespace StockPulse.Application.DTOs
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public decimal PriceThreshold { get; set; }
        public AlertType Type { get; set; } 
        public bool IsActive { get; set; }
    }
}
