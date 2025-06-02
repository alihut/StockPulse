using Microsoft.Extensions.Caching.Memory;
using StockPulse.Application.Interfaces;

namespace StockPulse.API.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            _memoryCache.Set(key, value, options);
        }

        public T? Get<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out var value) ? (T?)value : default;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
