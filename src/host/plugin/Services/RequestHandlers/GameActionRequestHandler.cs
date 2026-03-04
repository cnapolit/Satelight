using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Game = Playnite.SDK.Models.Game;

namespace HostPlugin.Services.RequestHandlers;

public abstract class GameActionRequestHandler<TReq, TRep>(ActionTracker actionTracker, GetGamesService getGamesService)
    where TReq : HostGameRequest where TRep : OpResponse, new()
{
    public TRep Handle(TReq request)
    {
        var game = getGamesService.GetGame(request);
        var op = actionTracker.AddOp(game.Id, RequestType);
        op.State = Func(game);
        return new() { Op = op };
    }

    protected abstract RequestType RequestType { get; }
    protected abstract OpState Func(Game game);
}