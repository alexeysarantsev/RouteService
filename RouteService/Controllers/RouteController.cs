using Microsoft.AspNetCore.Mvc;
using RouteService.Logic;
using RouteService.Model;
using RouteService.Model.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IAirlineProvider _airlineProvider;
        private readonly IAirportProvider _airportProvider;
        private readonly IRouteProvider _routeProvider;

        private readonly FlightsServiceProvider.IAirlineProviderFactory _airlineProviderFactory;
        private readonly FlightsServiceClient.IFlightsservice _flightsservice;

        public RouteController(IAirlineProvider airlineProvider, IAirportProvider airportProvider, IRouteProvider routeProvider
            /*, FlightsServiceProvider.IAirlineProviderFactory airlineProviderFactory, FlightsServiceClient.IFlightsservice flightsservice*/)
        {
            _airlineProvider = airlineProvider;
            _airportProvider = airportProvider;
            _routeProvider = routeProvider;

            //_airlineProviderFactory = airlineProviderFactory;
            //_flightsservice = flightsservice;
        }

        /// <summary>
        /// Searches for a route from the sourceAirport to the destinationAirport.
        /// </summary>
        /// <param name="sourceAirport">The name or the code of the source airport.</param>
        /// <param name="destinationAirport">The name or the code of the destination airport.</param>
        /// <returns>The route.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Journey), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        /// <response code="200">The route is found.</response>
        /// <response code="400">The source of destination airport is not set or not found.</response>
        /// <response code="404">The route is not found.</response>
        /// <response code="500">Unhandled server error.</response>
        public async Task<IActionResult> Get([FromQuery] string sourceAirport, [FromQuery] string destinationAirport, CancellationToken cancellationToken = default(CancellationToken))
        {
//            var aaa = _airlineProviderFactory.Get(_flightsservice);

            try
            {
                JourneyBuilder rb = new JourneyBuilder(_airlineProvider, _airportProvider, _routeProvider, sourceAirport, destinationAirport, cancellationToken);
                var routes = await rb.Build();
                if (routes == null)
                    return NotFound();
                return Ok(routes);
            }
            catch (AirportNotFoundException exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
