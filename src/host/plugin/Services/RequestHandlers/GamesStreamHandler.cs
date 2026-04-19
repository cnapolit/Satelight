using Common.Utility.Extensions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface;

namespace HostPlugin.Services.RequestHandlers;

public class GamesStreamHandler(IGetGamesService getGamesService) : IGamesStreamHandler
{
    public Task HandleRequestAsync(StreamGamesRequest request, IHostConnection connection, CancellationToken token)
        => getGamesService
          .GetGames()
          .While(connection.IsConnected)
          .WithCancellation(token)
          .ForEachAsync(g => connection.SendResponseAsync(new StreamGamesResponse { Game = g }, token));
}