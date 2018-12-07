using RouteService.Model.Interfaces;
using System;

namespace RouteService.Logic
{
    public class RouteBuilder
    {
        private readonly IAirlineProvider _airlineProvider;
        private readonly IAirportProvider _airportProvider;
        private readonly IRouteProvider _routeProvider;

        public RouteBuilder(IAirlineProvider airlineProvider, IAirportProvider airportProvider, IRouteProvider routeProvider)
        {
            _airlineProvider = airlineProvider ?? throw new ArgumentNullException(nameof(airlineProvider));
            _airportProvider = airportProvider ?? throw new ArgumentNullException(nameof(airportProvider));
            _routeProvider = routeProvider ?? throw new ArgumentNullException(nameof(routeProvider));
        }

        public void Build(string sourceAirport, string destinationAirport)
        {

        }
    }
}
