namespace StockPulse.Application.Interfaces
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        T? Get<T>(string key);
        void Remove(string key);
    }
}
