using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Threading;

namespace RouteService.FlightsServiceProvider
{
    public class AirlineProviderCached : GetByCodeCached<Airline>, IAirlineProvider
    {
        public AirlineProviderCached(TimeSpan ttl, IAirlineProvider airlineProvider) 
            : base(ttl, (alias, cancellationToken) => { return airlineProvider.Get(alias, cancellationToken); })
        {
        }
    }
}
