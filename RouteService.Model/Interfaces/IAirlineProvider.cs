﻿using RouteService.FlightsServiceClient.Models;
using System;
using System.Threading.Tasks;
    
namespace RouteService.Model.Interfaces
{
    public interface IAirlineProvider
    {
        Task<Airline> Get(string alias);
    }
}
