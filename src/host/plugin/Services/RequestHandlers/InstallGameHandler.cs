using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using Game = Playnite.SDK.Models.Game;

namespace HostPlugin.Services.RequestHandlers;

public class InstallGameHandler(IPlayniteAPI playniteApi, ActionTracker actionTracker, GetGamesService getGamesService)
    : GameActionRequestHandler<InstallGameRequest, InstallGameResponse>(actionTracker, getGamesService)
{
    protected override RequestType RequestType => RequestType.InstallGame;
    protected override OpState Func(Game game)
    {
        var state = OpState.Finished;
        if (!game.IsInstalled)
        {
            playniteApi.InstallGame(game.Id);
            state = OpState.Running;
        }
        return state;
    }
}