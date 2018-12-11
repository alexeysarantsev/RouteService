# RouteService
A service to search a route from one airport to another. 

Usage: GET  /api/route?sourceAirport={srcAirport}&destinationAirport={destAirport}
where srcAirport and destAirport are airport codes. Please see the swagger schema for more details. 
The service returns 200 with a Journey contains all routes (hubs) from the sourceAirport to the destinationAirport. 
Returns 404 if there is no routes from source to destination airports.
Returns 400 if sourceAirport or destinationAirport is not a valid airport alias.

Please note the service does not search the best (the shortest) route. It searches ANY route. 
The service uses the FlightService "https://homework.appulate.com/", but the URL could be amended in appsettings.json.

The services caches responses from the FlightService, default TTL is 30 min. I suppose the data provided by the FlightService is stable enough and could be cached. 







