using RouteService.Model.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class GetByCodeCached<TValue> : IGetByAlias<TValue>
    {
        private Cache.MemoryCache<string, TValue> _cache;
        private Func<string, CancellationToken, Task<TValue>> _factory;

        public GetByCodeCached(TimeSpan ttl, Func<string, CancellationToken, Task<TValue>> factory)
        {
            _cache = new Cache.MemoryCache<string, TValue>(ttl);
            _factory = factory;
        }

        public Task<TValue> Get(string alias, CancellationToken cancellationToken)
        {
            return _cache.GetOrCreateAsync(alias, (key) => { return _factory(key, cancellationToken); });
        }
    }
}
