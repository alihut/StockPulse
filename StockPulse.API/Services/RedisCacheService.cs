using StackExchange.Redis;
using StockPulse.Application.Interfaces;
using System.Text.Json;


namespace StockPulse.API.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            _db.StringSet(key, json, expiration);
        }

        public T? Get<T>(string key)
        {
            var value = _db.StringGet(key);
            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }
    }
}
