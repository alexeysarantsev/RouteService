using Models = RouteService.FlightsServiceClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteService.Model.Interfaces
{
    public interface IRouteProvider
    {
        Task<IList<Models.Route>> Get(string srcAirportCode);
    }
}
