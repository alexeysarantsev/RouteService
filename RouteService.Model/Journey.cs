using System;
using RouteService.FlightsServiceClient.Models;

namespace RouteService.Model
{
    /// <summary>
    /// Represens a journey contains one or more routes
    /// </summary>
    public class Journey
    {
        /// <summary>
        /// A routes array
        /// </summary>
        public Route[] Routes { get; set; }
    }
}
