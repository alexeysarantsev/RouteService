using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
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

        public async Task<IList<Route>> Get(string alias, CancellationToken cancellationToken = default(CancellationToken))
        {
            System.Diagnostics.Debug.WriteLine($"RouteProvider: {alias}");
            return await _flightsservice.ApiRouteOutgoingGetAsync(alias, cancellationToken);
        }
    }
}
