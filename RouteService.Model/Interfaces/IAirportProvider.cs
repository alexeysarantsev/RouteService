using RouteService.FlightsServiceClient.Models;
using System;
using System.Threading.Tasks;

namespace RouteService.Model.Interfaces
{
    public interface IAirportProvider : IGetByAlias<Airport>
    {
    }
}
