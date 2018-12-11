using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace RouteService.Cache
{
    /// <summary>
    /// A memory cache with default expiration time. The standard one does not provide it
    /// Only GetOrCreateAsync realized, no need to implement other ones.
    /// </summary>
    public class SimpleMemoryCache<TKey, TValue>
    {
        private TimeSpan _expirationTime;
        private IMemoryCache _memoryCache;
        public SimpleMemoryCache(TimeSpan expirationTime) : this (expirationTime, new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public SimpleMemoryCache(TimeSpan expirationTime, IMemoryCache memoryCache)
        {
            _expirationTime = expirationTime;
            _memoryCache = memoryCache;
        }

        public Task<TValue> GetOrCreateAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.Add(_expirationTime);
                return factory(key);
            });
        }
    }
}
