using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace RouteService.FlightsServiceProvider
{
    public class RouteProviderFactory : FlightServiceProviderFactoryCached<IRouteProvider, IList<Route>>, IRouteProviderFactory
    {
        public RouteProviderFactory(TimeSpan cacheLifetime)
            : base(cacheLifetime, (flightService, memoryCache) => { return new RouteProviderCached(memoryCache, new RouteProvider(flightService)); })
        { }
    }
}
