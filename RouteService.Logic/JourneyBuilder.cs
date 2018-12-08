using RouteService.FlightsServiceClient.Models;
using RouteService.Model;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.Logic
{
    public class JourneyBuilder
    {
        private readonly IAirlineProvider _airlineProvider;
        private readonly IAirportProvider _airportProvider;
        private readonly IRouteProvider _routeProvider;
        private readonly string _sourceAirportAlias;
        private readonly string _destinationAirportAlias;
        private readonly CancellationToken _cancellationToken;

        private ConcurrentDictionary<string, object> _processedAirports = new ConcurrentDictionary<string, object>();
        private ConcurrentDictionary<string, Task<Airline>> _activeAirlines = new ConcurrentDictionary<string, Task<Airline>>();

        private bool _found = false;

        public JourneyBuilder(IAirlineProvider airlineProvider, IAirportProvider airportProvider, IRouteProvider routeProvider,
            string sourceAirportAlias, string destinationAirportAlias, CancellationToken cancellationToken = default(CancellationToken))
        {
            _airlineProvider = airlineProvider ?? throw new ArgumentNullException(nameof(airlineProvider));
            _airportProvider = airportProvider ?? throw new ArgumentNullException(nameof(airportProvider));
            _routeProvider = routeProvider ?? throw new ArgumentNullException(nameof(routeProvider));
            _sourceAirportAlias = sourceAirportAlias ?? throw new ArgumentNullException(nameof(sourceAirportAlias)); 
            _destinationAirportAlias = destinationAirportAlias ?? throw new ArgumentNullException(nameof(destinationAirportAlias));
            _cancellationToken = cancellationToken;
        }

        public async Task<Journey> Build()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            if (string.Equals(_sourceAirportAlias, _destinationAirportAlias, StringComparison.InvariantCultureIgnoreCase))
                return new Journey { Routes = new Route[] { } };

            var sourceAirport = await _airportProvider.Get(_sourceAirportAlias, _cancellationToken);
            if (sourceAirport == null)
                throw new AirportNotFoundException($"Airport {_sourceAirportAlias} not found.");
            _cancellationToken.ThrowIfCancellationRequested();
            var destinationAirport = await _airportProvider.Get(_destinationAirportAlias, _cancellationToken);
            if (destinationAirport == null)
                throw new AirportNotFoundException($"Airport {_destinationAirportAlias} not found.");

            List<Route> routes = new List<Route>();
            var currentPoint = new RoutePoint { Airport = sourceAirport.Alias };

            var routePoint = await FindRoute(currentPoint, destinationAirport.Alias);
            if (routePoint == null)
                return null;
            
            return new Journey { Routes = ConvertRoutePointToRouts(routePoint) };
        }

        private async Task<RoutePoint> FindRoute(RoutePoint currentPoint, string destinationAirport)
        {
            if (_found)
                return null;
            _cancellationToken.ThrowIfCancellationRequested();
            //lets skip it if it has been processed already
            if (_processedAirports.GetOrAdd(currentPoint.Airport, currentPoint) != currentPoint)
                return null;

            var accessibleAirports = await _routeProvider.Get(currentPoint.Airport, _cancellationToken);

            // filter out already processed points
            var notProcessedAirports = accessibleAirports.Where(aa => !_processedAirports.ContainsKey(aa.DestAirport));

            ConcurrentBag<FlightsServiceClient.Models.Route> activeAirports = new ConcurrentBag<FlightsServiceClient.Models.Route>();
            var activeAirportTasks = notProcessedAirports.Select(async npa => {
                if (await IsAirlineActive(npa.Airline))
                    activeAirports.Add(npa);
            });
            await Task.WhenAll(activeAirportTasks);
            _cancellationToken.ThrowIfCancellationRequested();
            RoutePoint routePoint = null;
            var route = activeAirports.Where(npa => npa.DestAirport == destinationAirport).FirstOrDefault();
            if (route != null)
            {
                routePoint = new RoutePoint() { Airport = route.DestAirport, Route = route, Previous = currentPoint };
                _found = true;
            }
            else
            {
                var tasks = activeAirports.Select(async npa =>  {
                    var activeAirportRoutePoint = new RoutePoint() { Airport = npa.DestAirport, Route = npa, Previous = currentPoint };
                    var foundRoute =  await FindRoute(activeAirportRoutePoint, destinationAirport);
                    if (foundRoute != null && routePoint == null)
                        routePoint = foundRoute;
                });
                await Task.WhenAll(tasks);
            }
            return routePoint;
        }

        private async Task<bool> IsAirlineActive(string airlineCode)
        {
            var task = _activeAirlines.GetOrAdd(airlineCode,  (ac) => { return _airlineProvider.Get(ac, _cancellationToken); });
            var airline = await task;
            return airline.Active == true;
        }

        private Route[] ConvertRoutePointToRouts(RoutePoint routePoint)
        {
            int numberOfPoints = 0;
            RoutePoint current = routePoint;
            while (current != null && current.Route != null)
            {
                numberOfPoints++;
                current = current.Previous;
            }
            current = routePoint;
            Route[] routes = new Route[numberOfPoints];
            int i = numberOfPoints - 1;
            while (current != null && current.Route != null)
            {
                routes[i] = current.Route;
                current = current.Previous;
                i--;
            }
            return routes;
        }
    }

    public class RoutePoint
    {
        public string Airport { get; set; }
        public Route Route { get; set; }
        public RoutePoint Previous { get; set; }
    }
}
