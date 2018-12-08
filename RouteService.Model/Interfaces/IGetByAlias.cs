using System;
using System.Threading;
using System.Threading.Tasks;

namespace RouteService.Model.Interfaces
{
    public interface IGetByAlias<TValue>
    {
        Task<TValue> Get(string alias, CancellationToken cancellationToken = default(CancellationToken));
    }
}
