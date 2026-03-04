using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetOpHandler(ActionTracker actionTracker)
{
    public GetOpResponse Handle(GetOpRequest request) => new() { Op = actionTracker.GetOp(request.Id) };
}