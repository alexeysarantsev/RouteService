using Microsoft.Rest;
using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.FlightsServiceProvider
{
    public class AirportProvider : IAirportProvider
    {
        private IFlightsservice _flightsservice;

        public AirportProvider(IFlightsservice flightsservice)
        {
            _flightsservice = flightsservice ?? throw new ArgumentNullException(nameof(flightsservice));
        }

        public async Task<Airport> Get(string alias, CancellationToken cancellationToken = default(CancellationToken))
        {
            Airport airport = null;
            IList<Airport> airports = null;
            try
            {
                airports = await _flightsservice.ApiAirportSearchGetAsync(alias, cancellationToken: cancellationToken);
            }
            catch (HttpOperationException exeption)
            {
                if (exeption.Response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                    throw;
            }
            airport = airports?.Where(a => String.Equals(a.Alias, alias, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return airport;
        }
    }
}
