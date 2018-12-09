using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public class AirportProviderFactory : FlightServiceProviderFactoryCached<IAirportProvider, Airport>, IAirportProviderFactory
    {
        public AirportProviderFactory(TimeSpan cacheLifetime)
            : base(cacheLifetime, (flightService, memoryCache) => { return new AirportProviderCached(memoryCache, new AirportProvider(flightService)); })
        { }
    }
}
