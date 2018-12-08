using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RouteService.FlightsServiceClient.Models;
using RouteService.Model;
using RouteService.Model.Interfaces;

namespace RouteService.Logic.Tests
{
    [TestFixture]
    public class RouteBuilderTest
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
                new Airport() { Alias = "VKO"},
                new Airport() { Alias = "BAX"},
                new Airport() { Alias = "KZN"},
                new Airport() { Alias = "KGD"},
                new Airport() { Alias = "NRA"}, //no route airport
                new Airport() { Alias = "IAA"} //inactive airlines airport
            };
        }

        private void SetupAirlines()
        {
            _airlines = new Airline[]
            {
                new Airline() { Alias = "SU", Active = true},
                new Airline() { Alias = "S7", Active = true},
                new Airline() { Alias = "DP", Active = true},
                new Airline() { Alias = "UN", Active = false},
                new Airline() { Alias = "KD", Active = null},
            };
        }

        private void SetupRoutes()
        {
            _routes = new Route[]
            {
                new Route() {SrcAirport = "SVO", DestAirport = "KGD", Airline = "SU"}, new Route() {SrcAirport = "KGD", DestAirport = "SVO", Airline = "SU"},
                new Route() {SrcAirport = "VKO", DestAirport = "KGD", Airline = "S7"}, new Route() {SrcAirport = "KGD", DestAirport = "VKO", Airline = "S7"},
                new Route() {SrcAirport = "LED", DestAirport = "KGD", Airline = "KD"}, new Route() {SrcAirport = "KGD", DestAirport = "LED", Airline = "KD"},

                new Route () {SrcAirport = "SVO", DestAirport = "IAA", Airline = "UN" },
                new Route () {SrcAirport = "SVO", DestAirport = "IAA", Airline = "KD" },

                new Route() {SrcAirport = "SVO", DestAirport = "LED", Airline = "SU"}, new Route() {SrcAirport = "LED", DestAirport = "SVO", Airline = "SU"},
                new Route() {SrcAirport = "VKO", DestAirport = "LED", Airline = "DP"}, new Route() {SrcAirport = "LED", DestAirport = "VKO", Airline = "DP"},

                new Route() {SrcAirport = "SVO", DestAirport = "KZN", Airline = "SU"}, new Route() {SrcAirport = "KZN", DestAirport = "SVO", Airline = "SU"},

                new Route() {SrcAirport = "VKO", DestAirport = "BAX", Airline = "DP"}, new Route() {SrcAirport = "BAX", DestAirport = "VKO", Airline = "DP"},
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

            _airportStub.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string airport, CancellationToken canellationToken) =>
            {
                return _airports.Where(a => string.Equals(a.Alias, airport, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });

            _airlineStub.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string airline, CancellationToken canellationToken) =>
            {
                return _airlines.Where(a => a.Alias == airline).FirstOrDefault();
            });

            _routeStub.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string srcAirportCode, CancellationToken canellationToken) =>
            {
                return _routes.Where(a => a.SrcAirport == srcAirportCode).ToList();
            });
        }

        [Test]
        public async Task EmptyRouteOnSameSrcAndDesc()
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, "ABC", "ABC");
            var journey = await journeyBuilder.Build();
            Assert.AreEqual(journey.Routes.Length, 0);
        }

        [Test]
        public void SrcAirportNotFoundExceptionTest()
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, "___", "LED");
            Assert.ThrowsAsync<AirportNotFoundException>(async () => { await journeyBuilder.Build(); });
        }

        [Test]
        public void DestAirportNotFoundExceptionTest()
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, "LED", "___");
            Assert.ThrowsAsync<AirportNotFoundException>(async () => { await journeyBuilder.Build(); });
        }

        [TestCase("SVO", "KGD")]
        [TestCase("LED", "KGD")]
        [TestCase("BAX", "KZN")]
        public async Task ExistingRoutesTest(string src, string dest)
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, src, dest);
            var journey = await journeyBuilder.Build();
            VerifyRoute(src, dest, journey);
        }

        [Test]
        [TestCase("bax", "kzn")]
        [TestCase("bax", "Bax")]
        public async Task CaseSensivityTest(string src, string dest)
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, src, dest);
            var journey = await journeyBuilder.Build();
            Assert.IsNotNull(journey);
        }

        [Test]
        public async Task NoRouteAirportTest()
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, "SVO", "NRA");
            var journey = await journeyBuilder.Build();
            Assert.IsNull(journey);
        }

        [Test]
        public async Task InactiveAirlinesAirportTest()
        {
            JourneyBuilder journeyBuilder = new JourneyBuilder(_airlineStub.Object, _airportStub.Object, _routeStub.Object, "SVO", "IAA");
            var journey = await journeyBuilder.Build();
            Assert.IsNull(journey);
        }

        private bool IsAirlineActive(string airline)
        {
            var found = _airlines.Where(a => a.Alias == airline).FirstOrDefault();
            return (found != null) && found.Active == true;
        }

        private void VerifyRoute(string srcAirport, string destAirport, Journey journey)
        {
            Assert.AreEqual(srcAirport, journey.Routes[0].SrcAirport);
            for (int i = 0; i < journey.Routes.Length; i++)
            {
                var current = journey.Routes[i];
                if (i < journey.Routes.Length - 1)
                    Assert.AreEqual(journey.Routes[i].DestAirport, journey.Routes[i + 1].SrcAirport);
                Assert.IsTrue(_routes.Where(r => r.SrcAirport == current.SrcAirport && r.DestAirport == current.DestAirport && r.Airline == current.Airline).Any());
                Assert.IsTrue(IsAirlineActive(journey.Routes[i].Airline));
            }
            var lastRoute = journey.Routes[journey.Routes.Length - 1];
            Assert.AreEqual(destAirport, lastRoute.DestAirport);
        }
    }
}

