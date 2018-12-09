using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;

namespace RouteService.FlightsServiceProvider
{
    public class AirlineProviderCached : GetByCodeCached<Airline>, IAirlineProvider
    {
        public AirlineProviderCached(Cache.MemoryCache<string, Airline> cache, IAirlineProvider airlineProvider)
            : base(cache, (alias, cancellationToken) => { return airlineProvider.Get(alias, cancellationToken); })
        {
        }

    }
}
