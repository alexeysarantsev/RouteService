using RouteService.Model.Interfaces;
using System;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class GetByCodeCached<TValue> : IGetByAlias<TValue>
    {
        private Cache.MemoryCache<string, TValue> _cache;
        private Func<string, Task<TValue>> _factory;

        public GetByCodeCached(TimeSpan ttl, Func<string, Task<TValue>> factory)
        {
            _cache = new Cache.MemoryCache<string, TValue>(ttl);
            _factory = factory;
        }

        public Task<TValue> Get(string alias)
        {
            return _cache.GetOrCreateAsync(alias, _factory);
        }
    }
}
