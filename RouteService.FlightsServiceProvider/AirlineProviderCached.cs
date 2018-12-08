using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class AirlineProviderCached : GetByCodeCached<Airline>, IAirlineProvider
    {
        public AirlineProviderCached(TimeSpan ttl, IAirlineProvider airlineProvider) : base(ttl, (alias) => { return airlineProvider.Get(alias); })
        {
        }
    }
}
