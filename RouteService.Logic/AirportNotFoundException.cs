using System;
using System.Collections.Generic;
using System.Text;

namespace RouteService.Logic
{
    public class AirportNotFoundException : Exception
    {
        public AirportNotFoundException(string message) : base(message)
        {
        }
    }
}
