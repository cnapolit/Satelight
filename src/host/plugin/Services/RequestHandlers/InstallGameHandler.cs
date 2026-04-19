using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Game = Playnite.Game;

namespace HostPlugin.Services.RequestHandlers;

public class InstallGameHandler(IPlayniteApi playniteApi, IActionTracker actionTracker, ILibraryApi libraryApi)
    : GameActionRequestHandler<InstallGameRequest, InstallGameResponse>(actionTracker, libraryApi)
{
    protected override RequestType RequestType => RequestType.InstallGame;
    protected override async ValueTask<OpState> FuncAsync(Game game)
    {
        var state = OpState.Finished;
        if (game.InstallState is InstallState.Uninstalled)
        {
            await playniteApi.InstallGameAsync(game);
            state = OpState.Running;
        }
        return state;
    }
}