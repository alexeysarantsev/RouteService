﻿using RouteService.FlightsServiceClient;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Airport> Get(string alias)
        {
            return (await _flightsservice.ApiAirportSearchGetAsync(alias)).Where(a => a.Alias == alias).FirstOrDefault();
        }
    }
}