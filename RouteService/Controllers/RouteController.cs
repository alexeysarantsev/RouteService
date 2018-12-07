using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RouteService.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ApiController]
    public class RouteController : ControllerBase
    {
        public RouteController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string from, [FromQuery] string to)
        {
            return Ok();
        }
    }
}
