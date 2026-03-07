using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public interface IRequestHandler<TReq, TRep>
{
    ValueTask<TRep> HandleAsync(TReq request, CancellationToken token);
}