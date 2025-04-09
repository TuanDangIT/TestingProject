using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ResultPatternTesting.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T?> GetAsync<T>(string cacheKey)
        {
            
            string? cachedValue = await _distributedCache.GetStringAsync(cacheKey);
            if(cachedValue is null)
            {
                return default(T);
            }
            T? value = JsonConvert.DeserializeObject<T>(cachedValue);
            return value;
        }

        public Task RemoveAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync<T>(string cacheKey, T value)
        {
            var cfg = new DistributedCacheEntryOptions();
            cfg.SetAbsoluteExpiration(TimeSpan.FromSeconds(5));
            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(value)/*, cfg*/);
        }
    }
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string cacheKey);
        Task SetAsync<T>(string cacheKey, T value);
        Task RemoveAsync(string cacheKey);
    }
}
