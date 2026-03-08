using System;
using System.Collections.Generic;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services;

public interface IActionTracker
{
    Op AddOp(Guid localGameId, RequestType type, OpState state = OpState.Queued);
    void UpdateOp(Guid gameId, RequestType type, OpState state);
    Op? GetOp(Guid id);
    IList<Op> GetOps();
}
