using System;
using System.Net.Http;

namespace RouteService.FlightsServiceClient
{
    public partial class Flightsservice
    {
        public Flightsservice(Uri baseUri, HttpClient httpClient)
            : this(httpClient, disposeHttpClient: true)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }
    }
}
