using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using System.Threading;
using System.Threading.Tasks;
using Game = Playnite.SDK.Models.Game;

namespace HostPlugin.Services.RequestHandlers;

public abstract class GameActionRequestHandler<TReq, TRep>(IActionTracker actionTracker, IGetGamesService getGamesService) : RequestHandler<TReq>
    where TReq : HostGameRequest where TRep : OpResponse, new()
{
    public override async ValueTask<SatelightResponse> HandleAsync(TReq request, CancellationToken _)
    {
        var game = getGamesService.GetGame(request);
        var op = actionTracker.AddOp(game.Id, RequestType);
        op.State = Func(game);
        return new TRep { Op = op };
    }

    protected abstract RequestType RequestType { get; }
    protected abstract OpState Func(Game game);
}
