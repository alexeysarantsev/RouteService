using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public interface IFlightServiceProviderFactory<T> 
    {
        IGetByAlias<T> Get(FlightsServiceClient.IFlightsservice flightsservice);
    }


    public interface IAirlineProviderFactory //: IFlightServiceProviderFactory<Airline>
    {
        IAirlineProvider Get(FlightsServiceClient.IFlightsservice flightsservice);
    }

    public class AirlineProviderFactory : IAirlineProviderFactory
    {
        private TimeSpan _cacheLifetime;
        Cache.MemoryCache<string, Airline> _cache;

        public AirlineProviderFactory(TimeSpan cacheLifetime)
        {
            _cacheLifetime = cacheLifetime;
            _cache = new Cache.MemoryCache<string, Airline>(_cacheLifetime);
        }

        public IAirlineProvider Get(FlightsServiceClient.IFlightsservice flightsservice)
        {
            return new AirlineProviderCached(_cache, new AirlineProvider(flightsservice));
        }
    }
}
