using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace RouteService.FlightsServiceProvider
{
    public class RouteProviderCached : GetByCodeCached<IList<Route>>, IRouteProvider
    {
        public RouteProviderCached(TimeSpan ttl, IRouteProvider airportProvider) 
            : base(ttl, (alias, cancellationToken) => { return airportProvider.Get(alias, cancellationToken); })
        {
        }

        public RouteProviderCached(Cache.MemoryCache<string, IList<Route>> cache, IRouteProvider routeProvider)
           : base(cache, (alias, cancellationToken) => { return routeProvider.Get(alias, cancellationToken); })
        {
        }
    }

}
