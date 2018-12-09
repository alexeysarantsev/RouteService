using RouteService.Model.Interfaces;

namespace RouteService.FlightsServiceProvider
{
    public interface IRouteProviderFactory : IFlightServiceProviderFactory<IRouteProvider>
    {
    }

}
