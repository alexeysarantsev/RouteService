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
        private readonly string _sourceAirport;
        private readonly string _destinationAirport;
        private readonly CancellationToken _cancellationToken;

        private ConcurrentDictionary<string, object> _processedAirports = new ConcurrentDictionary<string, object>();
        private ConcurrentDictionary<string, bool> _activeAirlines = new ConcurrentDictionary<string, bool>();
        private ConcurrentDictionary<string, Task<Airline>> _activeAirlines2 = new ConcurrentDictionary<string, Task<Airline>>();

        private bool _found = false;

        public JourneyBuilder(IAirlineProvider airlineProvider, IAirportProvider airportProvider, IRouteProvider routeProvider,
            string sourceAirport, string destinationAirport, CancellationToken cancellationToken = default(CancellationToken))
        {
            _airlineProvider = airlineProvider ?? throw new ArgumentNullException(nameof(airlineProvider));
            _airportProvider = airportProvider ?? throw new ArgumentNullException(nameof(airportProvider));
            _routeProvider = routeProvider ?? throw new ArgumentNullException(nameof(routeProvider));
            _sourceAirport = sourceAirport;
            _destinationAirport = destinationAirport;
        }

        public async Task<Journey> Build()
        {
            if (_sourceAirport == _destinationAirport)
                return new Journey { Routes = new Route[] { } };

            if (await _airportProvider.Get(_sourceAirport) == null)
                throw new AirportNotFoundException($"Airport {_sourceAirport} not found.");
            if (await _airportProvider.Get(_destinationAirport) == null)
                throw new AirportNotFoundException($"Airport {_destinationAirport} not found.");

            List<Route> routes = new List<Route>();
            var currentPoint = new RoutePoint { Airport = _sourceAirport };

            var routePoint = await FindRoute(currentPoint, _destinationAirport);
            if (routePoint == null)
                return null;
            var stack = new Stack<RoutePoint>();
            var current = routePoint;
            while (current != null)
            {
                stack.Push(current);
                current = current.Previous;
            }
            while (stack.Any())
            {
                var currentRouteOint = stack.Pop();
                if (currentRouteOint.Route != null)
                    routes.Add(currentRouteOint.Route);
            }
            return new Journey { Routes = routes.ToArray() };
        }

        private async Task<RoutePoint> FindRoute(RoutePoint currentPoint, string destinationAirport)
        {
            if (_found)
                return null;
            if (_cancellationToken!= null && _cancellationToken.IsCancellationRequested)
                return null;
            //lets skip it if it has been processed already
            if (_processedAirports.GetOrAdd(currentPoint.Airport, currentPoint) != currentPoint)
                return null;

            var accessibleAirports = await _routeProvider.Get(currentPoint.Airport);

            // filter out already processed points
            var notProcessedAirports = accessibleAirports.Where(aa => !_processedAirports.ContainsKey(aa.DestAirport));

            ConcurrentBag<FlightsServiceClient.Models.Route> activeAirports = new ConcurrentBag<FlightsServiceClient.Models.Route>();
            var activeAirportTasks = notProcessedAirports.Select(async npa => {
                if (await IsAirlineActive2(npa.Airline))
                    activeAirports.Add(npa);
            });
            await Task.WhenAll(activeAirportTasks);
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
                    var routePoint1 = new RoutePoint() { Airport = npa.DestAirport, Route = npa, Previous = currentPoint };
                    var foundRoute =  await FindRoute(routePoint1, destinationAirport);
                    if (foundRoute != null && routePoint == null)
                        routePoint = foundRoute;
                });
                await Task.WhenAll(tasks);
            }
            return routePoint;
        }

        private async Task<bool> IsAirlineActive(string airlineCode)
        {
            if (!_activeAirlines.ContainsKey(airlineCode))
            {
                var airline = await _airlineProvider.Get(airlineCode);
                bool isActive = airline.Active == true;
                _activeAirlines[airlineCode] = isActive;
                return isActive;
            }
            return _activeAirlines[airlineCode];
        }

        private async Task<bool> IsAirlineActive2(string airlineCode)
        {
            var task = _activeAirlines2.GetOrAdd(airlineCode,  (ac) => { return _airlineProvider.Get(ac); });
            var airline = await task;
            return airline.Active == true;
        }
    }


    public class RoutePoint
    {
        public string Airport { get; set; }
        public Route Route { get; set; }
        public RoutePoint Previous { get; set; }
    }
}
