using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetOpHandler(IActionTracker actionTracker) : RequestHandler<GetOpRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetOpRequest request, CancellationToken _)
        => new GetOpResponse { Op = actionTracker.GetOp(request.Id) };
}