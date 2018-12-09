using System;
using System.Net.Http;

namespace RouteService.FlightsServiceClient
{
    public partial class Flightsservice
    {
        /// <summary>
        /// Custom ctor requires by DI
        /// </summary>
        public Flightsservice(Uri baseUri, HttpClient httpClient)
            : this(httpClient, disposeHttpClient: true)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }
    }
}
