using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class AirlineProvider : IAirlineProvider
    {
        private IFlightsservice _flightsservice;

        public AirlineProvider(IFlightsservice flightsservice)
        {
            _flightsservice = flightsservice ?? throw new ArgumentNullException(nameof(flightsservice));
        }
        public async Task<Airline> Get(string code)
        {
            return (await _flightsservice.ApiAirlineByAliasGetAsync(code)).Where(a => a.Alias == code).FirstOrDefault();
        }
    }
}
