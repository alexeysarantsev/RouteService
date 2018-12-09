using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace RouteService.FlightsServiceProvider
{
    public interface IRouteProviderFactory 
    {
        IRouteProvider Get(FlightsServiceClient.IFlightsservice flightsservice);
    }

    public class RouteProviderFactory : IRouteProviderFactory
    {
        private TimeSpan _cacheLifetime;
        Cache.MemoryCache<string, IList<Route>> _cache;

        public RouteProviderFactory(TimeSpan cacheLifetime)
        {
            _cacheLifetime = cacheLifetime;
            _cache = new Cache.MemoryCache<string, IList<Route>>(_cacheLifetime);
        }

        public IRouteProvider Get(FlightsServiceClient.IFlightsservice flightsservice)
        {
            return new RouteProviderCached(_cache, new RouteProvider(flightsservice));
        }
    }

}
