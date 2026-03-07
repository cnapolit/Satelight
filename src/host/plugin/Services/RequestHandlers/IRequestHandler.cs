using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public interface IRequestHandler
{
    ValueTask<SatelightResponse> HandleAsync(SatelightRequest request, CancellationToken token);
}

public interface IRequestHandler<TReq> : IRequestHandler
{
    ValueTask<SatelightResponse> HandleAsync(TReq request, CancellationToken token);
}