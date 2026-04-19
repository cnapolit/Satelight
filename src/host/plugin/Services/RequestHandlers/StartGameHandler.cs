using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Game = Playnite.Game;

namespace HostPlugin.Services.RequestHandlers;

public class StartGameHandler(IPlayniteApi playniteApi, IActionTracker actionTracker, ILibraryApi libraryApi)
    : GameActionRequestHandler<StartGameRequest, StartGameResponse>(actionTracker, libraryApi)
{
    protected override RequestType RequestType => RequestType.StartGame;
    protected override async ValueTask<OpState> FuncAsync(Game game)
    {
        var state = OpState.Finished;
        if (game.InstallState is not InstallState.Uninstalled)
        {
            await playniteApi.StartGameAsync(game);
            state = OpState.Queued;
        }
        return state;
    }
}