using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Linq;
using System.Threading;
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
        public async Task<Airline> Get(string alias, CancellationToken cancellationToken = default(CancellationToken))
        {
            System.Diagnostics.Debug.WriteLine($"AirlineProvider: {alias}");
            return (await _flightsservice.ApiAirlineByAliasGetAsync(alias, cancellationToken)).Where(a => a.Alias == alias).FirstOrDefault();
        }
    }
}
;