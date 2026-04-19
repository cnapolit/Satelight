using Comms.Common.Interface.Models;

namespace HostPlugin.Services;

public interface IActionTracker
{
    Op AddOp(string localGameId, RequestType type, OpState state = OpState.Queued);
    void UpdateOp(string gameId, RequestType type, OpState state);
    Op? GetOp(Guid id);
    IList<Op> GetOps();
}
