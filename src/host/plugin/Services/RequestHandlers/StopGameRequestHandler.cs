using Comms.Common.Interface.Models;
using Comms.Host.Interface;
using Comms.Host.Interface.Models;
using Playnite;
using System.Diagnostics;

namespace HostPlugin.Services.RequestHandlers;

public class StopGameRequestHandler(IActionTracker actionTracker, ILibraryApi libraryApi) : IStopGameRequestHandler
{
    public async Task HandleRequestAsync(StopGameRequest request, IHostConnection connection, CancellationToken token)
    {
        try
        {
            var game = libraryApi.Games.Get(request.Id) ?? throw new ArgumentException($"Game {request.Id} does not exist");
            var op = actionTracker.AddOp(game.Id, RequestType.StopGame);

            if (game.InstallState is not InstallState.Uninstalled)
            {
                op.State = OpState.Finished;
                await SendStopResponseAsync(connection, op, token);
                return;
            }

            using var process = Process.Start("playnite://NowPlaying/" + game.Id);
            if (process is null)
            {
                op.State = OpState.Failed;
                await SendStopResponseAsync(connection, op, token);
                return;
            }

            op.State = OpState.Running;
            await SendStopResponseAsync(connection, op, token);

            for (var i = 0; !process.HasExited && i < 10; i++)
            {
                await Task.Delay(1000, token);
            }

            if (op.State is not (OpState.Failed or OpState.Finished))
            {
                //game = getGamesService.GetGame(request);
                //op.State = game.IsRunning ? OpState.Failed : OpState.Finished;
            }

            if (!process.HasExited)
            {
                LogManager.GetLogger().Warn($"Process failed to stop after stop opAction '{op.State}' for game '{game.Id}'");
            }
        }
        catch (Exception e)
        {
            LogManager.GetLogger().Error(e, "Failed to perform StopGame");
            throw;
        }
    }

    private static Task SendStopResponseAsync(IHostConnection connection, Op op, CancellationToken token)
        => connection.SendResponseAsync(new StopGameResponse { Op = op }, token);
}