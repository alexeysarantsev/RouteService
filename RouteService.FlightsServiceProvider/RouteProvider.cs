using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class RouteProvider : IRouteProvider
    {
        private IFlightsservice _flightsservice;

        public RouteProvider(IFlightsservice flightsservice)
        {
            _flightsservice = flightsservice ?? throw new ArgumentNullException(nameof(flightsservice));
        }

        public async Task<IList<Route>> Get(string srcAirportCode)
        {
            return await _flightsservice.ApiRouteOutgoingGetAsync(srcAirportCode);
        }
    }
}
