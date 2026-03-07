using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public abstract class RequestHandler<TReq>: IRequestHandler<TReq> where TReq : SatelightRequest
{
    public async ValueTask<SatelightResponse> HandleAsync(SatelightRequest request, CancellationToken token)
        => await HandleAsync(request as TReq, token);
    public abstract ValueTask<SatelightResponse> HandleAsync(TReq request, CancellationToken token);
}
