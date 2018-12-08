using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace RouteService.FlightsServiceProvider
{
    public class RouteProviderCached : GetByCodeCached<IList<Route>>, IRouteProvider
    {
        public RouteProviderCached(TimeSpan ttl, IRouteProvider airportProvider) : base(ttl, (alias) => { return airportProvider.Get(alias); })
        {
        }
    }

}
