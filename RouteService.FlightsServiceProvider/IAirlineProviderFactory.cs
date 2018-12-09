using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public interface IAirlineProviderFactory : IFlightServiceProviderFactory<IAirlineProvider>
    {
    }
}
