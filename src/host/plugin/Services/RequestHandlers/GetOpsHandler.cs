using System.Linq;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetOpsHandler(ActionTracker actionTracker)
{
    public GetOpsResponse  Handle() => new() { Ops = actionTracker.GetOps().ToArray() };
}