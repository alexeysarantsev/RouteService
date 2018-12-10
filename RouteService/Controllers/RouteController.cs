using Microsoft.AspNetCore.Mvc;
using RouteService.Logic;
using RouteService.Model;
using RouteService.Model.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.Api.Controllers
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
        /// <param name="cancellationToken">Cancellation token.</param>
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
            if (string.IsNullOrEmpty(sourceAirport))
                return BadRequest("sourceAirport is empty");

            if (string.IsNullOrEmpty(destinationAirport))
                return BadRequest("destinationAirport is empty");

            Journey journey = null;
            try
            {
                JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineProvider, _airportProvider, _routeProvider, sourceAirport, destinationAirport, cancellationToken);
                journey = await journeyBuilder.Build();
            }
            catch (AirportNotFoundException exc)
            {
                return BadRequest(exc.Message);
            }
            if (journey == null)
                return NotFound();
            return Ok(journey);

        }
    }
}
