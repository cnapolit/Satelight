using Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;
using Server.Models.Database;
using Server.Models.Events;
using Server.Models.UserInterface;
using Server.Services.Host;
using HostGame = Server.Models.Database.HostGame;

namespace Server.Services;

public class SatelightDatabaseClient : IDatabaseClient
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly MediaFileService _mediaFileService;
    private readonly HostClient _hostClient;
    private readonly HostNetworkService _hostNetworkService;

    public SatelightDatabaseClient(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        MediaFileService mediaFileService,
        HostClient hostClient,
        HostNetworkService hostNetworkService)
    {
        _databaseContextFactory = databaseContextFactory;
        _mediaFileService = mediaFileService;
        _hostClient = hostClient;
        _hostNetworkService = hostNetworkService;
        _hostClient.GameUpdated += CallGameUpdated;
    }

    private Task CallGameUpdated(GameUpdatedEventArgs args)
        => GameUpdated?.Invoke(new()
        {
            GameInfo = GetGameInfo(args.Game),
            Type = args.Type,
            State = args.State
        }) ?? Task.CompletedTask;

    public event Func<GameDisplayUpdatedEventArgs, Task>? GameUpdated;

    public async Task<List<GameDisplayInfo>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await _databaseContextFactory.CreateDbContextAsync(cancellationToken);
        return await databaseContext.Games.Include(g => g.UserGameInfo).Where(g => g.UserGameInfo.Any(i => i.Favorite)).Select(g => new GameDisplayInfo
        {
            Id = g.Id.ToString().ToLower(),
            Name = g.Name,
            Cover = _mediaFileService.GetCovers(g.Id).FirstOrDefault(),
            Background = _mediaFileService.GetBackgrounds(g.Id).FirstOrDefault(),
            Logo = _mediaFileService.GetLogos(g.Id).FirstOrDefault()
        }).ToListAsync(cancellationToken);
    }

    public async Task<GameInfo> GetGameInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await _databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var gameId = Guid.Parse(id);
        var game = await databaseContext
                        .Games
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.HostGames)
                        .FirstAsync (g => g.Id == gameId, cancellationToken);
        _hostClient.UpdateGameStateAsync(game.Id, cancellationToken);
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

    private static GameInfo GetGameInfo(Game game) => new()
    {
        Id = game.Id.ToString(),
        Description = game.Description,
        Versions = game.GameVariants.SelectMany(g => g.LibraryGames).Select(g => new GameVersion
        {
            Id = g.Id.ToString(),
            Name = g.GameVariant.SubName,
            Status = GetStatus(g.HostGames.First())
        }).ToList()
    };

    private static GameStatus GetStatus(HostGame hostGame)
        => hostGame.Playing   ? GameStatus.Running
         : hostGame.Installed ? GameStatus.Installed
                              : GameStatus.NotInstalled;

    public Task StartGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(_hostClient.StartAsync, id, cancellationToken);
    public Task StopGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(_hostClient.StopAsync, id, cancellationToken);

    public Task InstallGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(_hostClient.InstallAsync, id, cancellationToken);

    public Task UninstallGameAsync(string id, CancellationToken cancellationToken = default)
        => ExecuteAsync(_hostClient.UninstallAsync, id, cancellationToken);

    public async Task<HostInfo> GetHostAsync(CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await _databaseContextFactory.CreateDbContextAsync(cancellationToken);
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
        await using var databaseContext = await _databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var hosts = await databaseContext.Hosts.AsNoTracking().ToListAsync(cancellationToken);
        var host = await hosts.FirstOrDefaultAsync(h => _hostNetworkService.HostIsActiveAsync(h.Ip))
            ?? throw new Exception("No Host Available");
        var gameId = Guid.Parse(id);
        var game = await databaseContext
                        .Games
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(l => l.HostGames)
                        .FirstAsync(g => g.Id == gameId, cancellationToken);
        var libraryGame = game.GameVariants.First().LibraryGames.First();
        var hostGame = libraryGame.HostGames.First(g => g.HostId == host.Id);
        
        await action(libraryGame.LibraryId, libraryGame.Id, hostGame.HostId, cancellationToken);
    }
}