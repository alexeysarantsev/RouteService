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
        public RouteController(IAirlineProvider airlineProvider, IAirportProvider airportProvider, IRouteProvider routeProvider)
        {
            _airlineProvider = airlineProvider;
            _airportProvider = airportProvider;
            _routeProvider = routeProvider;
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
            //var sourceAirportInfo = await _airportProvider.Get(sourceAirport);
            //if (sourceAirportInfo == null)
            //    return BadRequest($"Airport {sourceAirport} not found");
            //var destinationAirportInfo = await _airportProvider.Get(destinationAirport);
            //if (destinationAirportInfo == null)
            //    return BadRequest($"Airport {destinationAirportInfo} not found");
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
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
