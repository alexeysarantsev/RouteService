using RouteService.FlightsServiceClient;
using System;

namespace RouteService.FlightsServiceProvider
{
    public class FlightServiceProviderFactoryCached<TProvider, TObject> : IFlightServiceProviderFactory<TProvider>
    {
        private Cache.SimpleMemoryCache<string, TObject> _cache;
        private Func<IFlightsservice, Cache.SimpleMemoryCache<string, TObject>, TProvider> _createProviderFunc;

        public FlightServiceProviderFactoryCached(TimeSpan cacheLifetime, Func<IFlightsservice, Cache.SimpleMemoryCache<string, TObject>, TProvider> createProviderFunc)
        {
            _cache = new Cache.SimpleMemoryCache<string, TObject>(cacheLifetime);
            _createProviderFunc = createProviderFunc;
        }

        public TProvider Get(IFlightsservice flightsservice)
        {
            return _createProviderFunc(flightsservice, _cache);
        }
    }
}
