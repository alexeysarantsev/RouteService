using RouteService.FlightsServiceClient;
using System;

namespace RouteService.FlightsServiceProvider
{
    public class FlightServiceProviderFactoryCached<TProvider, TObject> : IFlightServiceProviderFactory<TProvider>
    {
        private Cache.MemoryCache<string, TObject> _cache;
        private Func<IFlightsservice, Cache.MemoryCache<string, TObject>, TProvider> _createProviderFunc;

        public FlightServiceProviderFactoryCached(TimeSpan cacheLifetime, Func<IFlightsservice, Cache.MemoryCache<string, TObject>, TProvider> createProviderFunc)
        {
            _cache = new Cache.MemoryCache<string, TObject>(cacheLifetime);
            _createProviderFunc = createProviderFunc;
        }

        public TProvider Get(IFlightsservice flightsservice)
        {
            return _createProviderFunc(flightsservice, _cache);
        }
    }
}
