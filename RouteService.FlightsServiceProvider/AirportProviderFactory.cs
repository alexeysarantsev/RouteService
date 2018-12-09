using Microsoft.Extensions.Caching.Memory;
using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RouteService.FlightsServiceProvider
{
        public interface IAirportProviderFactory //: IFlightServiceProviderFactory<Airline>
        {
            IAirportProvider Get(FlightsServiceClient.IFlightsservice flightsservice);
        }

        public class AirportProviderFactory : IAirportProviderFactory
    {
            private TimeSpan _cacheLifetime;
            Cache.MemoryCache<string, Airport> _cache;

            public AirportProviderFactory(TimeSpan cacheLifetime)
            {
                _cacheLifetime = cacheLifetime;
                _cache = new Cache.MemoryCache<string, Airport>(_cacheLifetime);
            }

            public IAirportProvider Get(FlightsServiceClient.IFlightsservice flightsservice)
            {
                return new AirportProviderCached(_cache, new AirportProvider(flightsservice));
            }
        }

}
