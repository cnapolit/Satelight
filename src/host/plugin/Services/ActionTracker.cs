using System;
using System.Collections.Generic;
using System.Linq;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services;

public class ActionTracker : IActionTracker
{
    private readonly object _pollLock = new();

    private readonly Dictionary<Guid, Op> _ops = [];
    public Op AddOp(Guid localGameId, RequestType type, OpState state = OpState.Queued)
    {
        lock (_pollLock)
        {
            Op op = new()
            {
                Id = Guid.NewGuid(),
                GameId = localGameId,
                Type = type,
                State = state
            };
            return _ops[op.Id] = op;
        }
    }

    public void UpdateOp(Guid gameId, RequestType type, OpState state)
    {
        lock (_pollLock)
        {
            var opEntry = _ops.Values.FirstOrDefault(
                o => o.GameId == gameId
                  && o.Type == type
                  && o.State is not (OpState.Finished or OpState.Failed));
            if (opEntry is not null)
            {
                opEntry.State = state;
                opEntry.LastUpdated = DateTime.UtcNow;
                return;
            }

            Op op = new()
            {
                GameId = gameId,
                Type = type,
                State = state
            };
            _ops[op.Id] = op;
        }
    }

    public Op? GetOp(Guid id)
    {
        lock (_pollLock)
        {
            return _ops.TryGetValue(id, out var opEntry) ? opEntry : null;
        }
    }

    public IList<Op> GetOps()
    {
        lock (_pollLock)
        {
            return _ops.Values.ToList();
        }
    }
}