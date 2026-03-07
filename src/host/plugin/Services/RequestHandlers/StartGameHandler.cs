using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using Game = Playnite.SDK.Models.Game;

namespace HostPlugin.Services.RequestHandlers;

public class StartGameHandler(IPlayniteAPI playniteApi, IActionTracker actionTracker, IGetGamesService getGamesService)
    : GameActionRequestHandler<StartGameRequest, StartGameResponse>(actionTracker, getGamesService)
{
    protected override RequestType RequestType => RequestType.StartGame;
    protected override OpState Func(Game game)
    {
        var state = OpState.Finished;
        if (!game.IsRunning)
        {
            playniteApi.StartGame(game.Id);
            state = OpState.Queued;
        }
        return state;
    }
}