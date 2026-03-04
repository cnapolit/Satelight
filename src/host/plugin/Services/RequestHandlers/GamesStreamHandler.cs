using System.Threading;
using System.Threading.Tasks;
using Common.Utility.Extensions;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GamesStreamHandler(GetGamesService getGamesService)
{
    public Task HandleRequestAsync(StreamGamesRequest request, ISatelightConnection connection, CancellationToken token)
        => getGamesService
          .GetGames()
          .While(connection.IsConnected)
          .WithCancellation(token)
          .ForEachAsync(g => connection.SendResponseAsync(new StreamGamesResponse { Game = g }, token));
}