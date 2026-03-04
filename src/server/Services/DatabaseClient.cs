using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;
using Server.Models.UserInterface;
using Server.Services.Host;
using HostGame = Server.Models.Database.HostGame;

namespace Server.Services;

public class SatelightDatabaseClient(IDbContextFactory<DatabaseContext> databaseContextFactory, MediaFileService mediaFileService, HostClient hostClient) : IDatabaseClient
{
    public async Task<List<GameDisplayInfo>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        return await databaseContext.Games.Include(g => g.UserGameInfo).Where(g => g.UserGameInfo.Any(i => i.Favorite)).Select(g => new GameDisplayInfo
        {
            Id = g.Id.ToString(),
            Name = g.Name,
            Cover = mediaFileService.GetCovers(g.Id).FirstOrDefault(),
            Background = mediaFileService.GetBackgrounds(g.Id).FirstOrDefault()
        }).ToListAsync(cancellationToken);
    }

    public async Task<GameInfo> GetGameInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var gameId = Guid.Parse(id);
        var game = await databaseContext
                        .Games
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.HostGames)
                        .FirstAsync (g => g.Id == gameId, cancellationToken);
        hostClient.UpdateGameStateAsync(game.Id, cancellationToken);
        return new()
        {
            Id = id,
            Description = game.Description,
            Versions = game.GameVariants.SelectMany(g => g.LibraryGames).Select(g => new GameVersion
            {
                Id = g.Id.ToString(),
                Name = g.GameVariant.SubName,
                Status = GetStatus(g.HostGames.First())
            }).ToList()
        };
    }

    private static GameStatus GetStatus(HostGame hostGame)
        => hostGame.Playing   ? GameStatus.Running
         : hostGame.Installed ? GameStatus.Installed
                              : GameStatus.NotInstalled;

    public Task StartGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(hostClient.StartAsync, id, cancellationToken);
    public Task StopGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(hostClient.StopAsync, id, cancellationToken);

    public Task InstallGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(hostClient.InstallAsync, id, cancellationToken);

    public Task UninstallGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(hostClient.UninstallAsync, id, cancellationToken);

    public async Task<HostInfo> GetHostAsync(CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var host = await databaseContext.Hosts.FirstAsync(cancellationToken);
        return new()
        {
            Id = host.Id.ToString(),
            Name = host.HostName,
            Ip = host.Ip,
            Port = host.Port
        };
    }

    public async Task ExecuteAsync(
        Func<Guid, Guid, Guid, CancellationToken, Task<Op>> action,
        string id,
        CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var gameId = Guid.Parse(id);
        var game = await databaseContext
                        .Games
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(l => l.HostGames)
                        .FirstAsync(g => g.Id == gameId, cancellationToken);
        var libraryGame = game.GameVariants.First().LibraryGames.First();
        var hostGame = libraryGame.HostGames.First();
        
        await action(libraryGame.LibraryId, libraryGame.Id, hostGame.HostId, cancellationToken);
    }
}