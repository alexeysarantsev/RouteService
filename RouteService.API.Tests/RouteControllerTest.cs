using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RouteService.Controllers;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model;
using RouteService.Model.Interfaces;


namespace RouteService.API.Tests
{
    public class RouteControllerTest
    {
        
        private IEnumerable<Airport> _airports;
        private IEnumerable<Airline> _airlines;
        private IEnumerable<Route> _routes;

        private Mock<IAirportProvider> _airportStub;
        private Mock<IAirlineProvider> _airlineStub;
        private Mock<IRouteProvider> _routeStub;

        private void SetupAirports()
        {
            _airports = new Airport[]
            {
                new Airport() { Alias = "LED"},
                new Airport() { Alias = "SVO"},
            };
        }

        private void SetupAirlines()
        {
            _airlines = new Airline[]
            {
                new Airline() { Alias = "SU", Active = true},
            };
        }

        private void SetupRoutes()
        {
            _routes = new Route[]
            {
                new Route() {SrcAirport = "SVO", DestAirport = "LED", Airline = "SU"}
            };
        }

        [SetUp]
        public void Setup()
        {
            SetupAirlines();
            SetupAirports();
            SetupRoutes();

            _airportStub = new Mock<IAirportProvider>();
            _airlineStub = new Mock<IAirlineProvider>();
            _routeStub = new Mock<IRouteProvider>();

            _airportStub.Setup(a => a.Get(It.IsAny<string>())).ReturnsAsync((string airport) =>
            {
                return _airports.Where(a => a.Alias == airport).FirstOrDefault();
            });

            _airlineStub.Setup(a => a.Get(It.IsAny<string>())).ReturnsAsync((string airline) =>
            {
                return _airlines.Where(a => a.Alias == airline).FirstOrDefault();
            });

            _routeStub.Setup(a => a.Get(It.IsAny<string>())).ReturnsAsync((string srcAirportCode) =>
            {
                return _routes.Where(a => a.SrcAirport == srcAirportCode).ToList();
            });
        }
        

        [Test]
        public async Task EmptySourceNotFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get(string.Empty, "LED");
            Assert.IsAssignableFrom(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task EmptyDestNotFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get("LED", string.Empty);
            Assert.IsAssignableFrom(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task SourceNotFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get("___", "LED");
            Assert.IsAssignableFrom(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task DestNotFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get("LED", "___");
            Assert.IsAssignableFrom(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task RouteNotFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get("LED", "SVO");
            Assert.IsAssignableFrom(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task RouteFoundTest()
        {
            RouteController controller = new RouteController(_airlineStub.Object, _airportStub.Object, _routeStub.Object);
            var result = await controller.Get("SVO", "LED");
            Assert.IsAssignableFrom(typeof(OkObjectResult), result);
            Assert.IsAssignableFrom(typeof(Journey), (result as OkObjectResult).Value);
        }
    }
}
