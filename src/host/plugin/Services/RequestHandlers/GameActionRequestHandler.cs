using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Game = Playnite.Game;

namespace HostPlugin.Services.RequestHandlers;

public abstract class GameActionRequestHandler<TReq, TRep>(IActionTracker actionTracker, ILibraryApi libraryApi) : RequestHandler<TReq>
    where TReq : HostGameRequest where TRep : OpResponse, new()
{
    public override async ValueTask<SatelightResponse> HandleAsync(TReq request, CancellationToken _)
    {
        
        var game = libraryApi.Games.Get(request.Id) ?? throw new ArgumentException($"Game {request.Id} does not exist");
        var op = actionTracker.AddOp(game.Id, RequestType);
        op.State = await FuncAsync(game);
        return new TRep { Op = op };
    }

    protected abstract RequestType RequestType { get; }
    protected abstract ValueTask<OpState> FuncAsync(Game game);
}
