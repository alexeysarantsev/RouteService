using RouteService.Model.Interfaces;
using System;

namespace RouteService.FlightsServiceProvider
{
    public interface IAirportProviderFactory : IFlightServiceProviderFactory<IAirportProvider>
    {
    }
}
