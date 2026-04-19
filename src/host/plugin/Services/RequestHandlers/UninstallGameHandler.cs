using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Game = Playnite.Game;

namespace HostPlugin.Services.RequestHandlers;

public class UninstallGameHandler(IPlayniteApi playniteApi, IActionTracker actionTracker, ILibraryApi libraryApi)
    : GameActionRequestHandler<UninstallGameRequest, UninstallGameResponse>(actionTracker, libraryApi)
{
    protected override RequestType RequestType => RequestType.InstallGame;
    protected override async ValueTask<OpState> FuncAsync(Game game)
    {
        var state = OpState.Finished;
        if (game.InstallState is InstallState.Installed)
        {
            await playniteApi.UninstallGameAsync(game);
            state = OpState.Running;
        }
        return state;
    }
}