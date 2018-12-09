using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RouteService.FlightsServiceProvider
{
    public interface IFlightServiceProvider
    {
        IAirlineProvider GetAirlineProvider();
        IAirportProvider GetAirportProvider();
        IRouteProvider GetRouteProvider();

    }
}
