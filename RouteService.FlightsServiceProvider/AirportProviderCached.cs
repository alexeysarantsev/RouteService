using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public class AirportProviderCached : GetByCodeCached<Airport>, IAirportProvider
    {
        public AirportProviderCached(TimeSpan ttl, IAirportProvider airportProvider) : base(ttl, (alias) => { return airportProvider.Get(alias); })
        {
        }
    }
}
