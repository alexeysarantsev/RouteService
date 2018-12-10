# RouteService
A service to search a route from one airport to another. 

Usage: GET  /api/route?sourceAirport={srcAirport}&destinationAirport={destAirport}
where srcAirport and destAirport are airport codes. Please see the swagger schema for more details. 
The service returns a Journey contains all routes (hubs) from the sourceAirport to the destinationAirport. 

Please note the service does not search the best (the shortest) route. It searches any route. 
The service uses the FlightService "https://homework.appulate.com/", but the URL could be amended in appsettings.json.

The services caches responses from the FlightService, default TTL is 30 min. I suppose the data provided by the FlightService is stable enough and could be cached. 







Some links 

Using CancellationTokens in ASP.NET Core MVC controllers
https://andrewlock.net/using-cancellationtokens-in-asp-net-core-mvc-controllers/ 

https://docs.microsoft.com/ru-ru/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly

HttpClientFactory in ASP.NET Core 2.1 (Part 2)
https://www.stevejgordon.co.uk/httpclientfactory-named-typed-clients-aspnetcore

Integrating Autorest Clients with HttpClientFactory and DI
http://michaco.net/blog/IntegratingAutorestWithHttpClientFactoryAndDI