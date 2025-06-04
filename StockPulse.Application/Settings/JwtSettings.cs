namespace StockPulse.Application.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; }
        public string Issuer { get; set; } = "StockPulse";
        public string Audience { get; set; } = "StockPulseUsers";
    }

}
