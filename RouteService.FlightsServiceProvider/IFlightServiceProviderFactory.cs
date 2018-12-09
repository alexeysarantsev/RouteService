using System;

namespace RouteService.FlightsServiceProvider
{
    public interface IFlightServiceProviderFactory<TProvider>
    {
        TProvider Get(FlightsServiceClient.IFlightsservice flightsservice);
    }
}
