using StockPulse.Domain.Enums;

namespace StockPulse.Application.DTOs
{
    public class CreateAlertRequestDto
    {
        public Guid UserId { get; set; }
        public string Symbol { get; set; }
        public decimal PriceThreshold { get; set; }
        public AlertType Type { get; set; }
    }

}
