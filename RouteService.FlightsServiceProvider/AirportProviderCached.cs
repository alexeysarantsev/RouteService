using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public class AirportProviderCached : GetByCodeCached<Airport>, IAirportProvider
    {
        public AirportProviderCached(Cache.MemoryCache<string, Airport> cache, IAirportProvider airportProvider)
            : base(cache, (alias, cancellationToken) => { return airportProvider.Get(alias, cancellationToken); })
        {
        }
    }
}
