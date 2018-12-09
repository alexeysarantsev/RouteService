using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace RouteService.FlightsServiceProvider
{
    public class AirlineProviderFactory : FlightServiceProviderFactoryCached<IAirlineProvider, Airline>, IAirlineProviderFactory
    {
        public AirlineProviderFactory(TimeSpan cacheLifetime)
            : base(cacheLifetime, (flightService, memoryCache) => { return new AirlineProviderCached(memoryCache, new AirlineProvider(flightService));})
        { }
    }
}
