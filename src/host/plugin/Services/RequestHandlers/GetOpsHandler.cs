using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetOpsHandler(IActionTracker actionTracker) : RequestHandler<GetOpsRequest>
{
    public async override ValueTask<SatelightResponse> HandleAsync(GetOpsRequest request, CancellationToken _)
        => new GetOpsResponse { Ops = actionTracker.GetOps().ToArray() };
}