using Server.Models.Events;
using Server.Models.UserInterface;

namespace Server.Services;

public class PlayniteWebDatabaseClient : IDatabaseClient
{
    public event Func<GameDisplayUpdatedEventArgs, Task> GameUpdated;

    public async Task<List<GameDisplayInfo>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        // List<GameDisplayInfo> games = [];
        // var usersResult = await playniteWebClient.GetUsers.ExecuteAsync(cancellationToken);
        // foreach (var user in usersResult.Data.Users.Users)
        // {
        //     var gamesResult = await playniteWebClient.GetGames.ExecuteAsync(user.Username, cancellationToken);
        //     games.AddRange(gamesResult.Data.Libraries.SelectMany(l => l.Games.Select(g => 
        //     {
        //         var primaryRelease = g.PrimaryRelease is null ? g.Releases[0] : g.Releases.First(r => r.Id == g.PrimaryRelease.Id);
        //         return new GameDisplayInfo
        //         {
        //             // Id = game.Id,
        //             Name = primaryRelease.Title,
        //             Cover = primaryRelease.Cover,
        //         };
        //     })));
        // }
        // return games;
        throw new NotImplementedException();
    }

    public Task<GameInfo> GetGameInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StartGameAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StopGameAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task InstallGameAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UninstallGameAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<HostInfo> GetHostAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}