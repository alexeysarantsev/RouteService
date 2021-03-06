﻿using RouteService.Model.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class GetByCodeCached<TValue> : IGetByAlias<TValue>
    {
        private Cache.SimpleMemoryCache<string, TValue> _cache;
        private Func<string, CancellationToken, Task<TValue>> _factory;

        public GetByCodeCached(TimeSpan ttl, Func<string, CancellationToken, Task<TValue>> factory)
        {
            _cache = new Cache.SimpleMemoryCache<string, TValue>(ttl);
            _factory = factory;
        }

        public GetByCodeCached(Cache.SimpleMemoryCache<string, TValue> cache, Func<string, CancellationToken, Task<TValue>> factory)
        {
            _cache = cache;
            _factory = factory;
        }

        public Task<TValue> Get(string alias, CancellationToken cancellationToken)
        {
            return _cache.GetOrCreateAsync(alias, (key) => { return _factory(key, cancellationToken); });
        }
    }
}
