using System.Net;
using System.Net.NetworkInformation;
using Common.Utility.Extensions;
using Common.Utility.Functions;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;
using Server.Models.Database;
using Server.Models.Events;
using Server.Models.Host;
using HostGame = Server.Models.Database.HostGame;

namespace Server.Services.Host;

public class GameLibraryService(
    ILogger<GameLibraryService> logger,
    IDbContextFactory<DatabaseContext> databaseContextFactory,
    HostClient hostClient,
    MediaFileService mediaFileService)
{
    public event Func<GameUpdatedEventArgs, Task>? GameUpdated;

    public async Task GetGamesAsync(
        Action<string, int, int> progressCallback, Action<int, int> stepProgressCallback, List<Guid>? hostsToUpdates = null, CancellationToken token = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(token);
        
        var hostQuery = hostsToUpdates is null
            ? databaseContext.Hosts
            : databaseContext.Hosts.Where(h => hostsToUpdates.Contains(h.Id));
        var hosts = await hostQuery.ToListAsync(token);
        var progress = 0;
        var total = hosts.Count + 1;
        var user = await databaseContext.Users.FirstAsync(token);
        var completionStatus = await databaseContext.CompletionStatuses.FirstAsync(token);

        List<Task> mediaDownloadTasks = [];

        var defaultGameVariantType = await databaseContext.GameVariantTypes.FirstAsync(
            v => v.GameVariantFlags.HasFlag(GameVariantFlags.Default), token);
        foreach (var host in hosts) try
        {
            progressCallback($"Retrieving games from host '{host.DisplayName}'", progress++, total);
            var cache = await GetHostCacheAsync(host, databaseContext, token);
            var count = await hostClient.GetGameCountAsync(host, token);
            var stepProgress = 0;
            await foreach (var protoGame in hostClient.GetGamesAsync(host, token))
            {
                stepProgressCallback(stepProgress++, count);
                Dictionary<Guid, HostGame> hostGames = [];
                List<(Satelight.Protos.Host.HostGame, LibraryGame?, Library)> newHostGames = [];
                foreach (var hostGame in protoGame.HostGames) try
                {
                    var hostGameId = hostGame.Id.ToGuid().ToString();
                    var databaseHostGame = await databaseContext
                        .HostGames
                        .Include(h => h.LibraryGame)
                        .ThenInclude(l => l.GameVariant)
                        .ThenInclude(v => v.Games)
                        .FirstOrDefaultAsync(
                            h => h.HostId == host.Id
                              && h.HostGameId == hostGameId,
                            cancellationToken: token);
                    if (databaseHostGame != null)
                    {
                        hostGames[databaseHostGame.Id] = databaseHostGame;
                        continue;
                    }

                    Library library;
                    if (hostGame.Library.IsEmpty)
                    {
                        library = await databaseContext.Libraries.FindAsync([Guid.Empty], cancellationToken: token)
                                    ?? throw new Exception("Default library not found");
                    }
                    else
                    {
                        var libraryName = cache.Libraries.First(l => l.Id == hostGame.Library).Name;
                        library = await databaseContext.Libraries.AsNoTracking().FirstAsync(
                            l => l.Name == libraryName, cancellationToken: token);
                    }

                    var libraryGame = await databaseContext.LibraryGames.FirstOrDefaultAsync(
                        l => l.LibraryId == library.Id && l.LibraryGameId == hostGame.LibraryGameId,
                        cancellationToken: token);
                    newHostGames.Add((hostGame, libraryGame, library));
                }
                catch (Exception e)
                {
                    logger.LogError($"Error processing host game instance '{hostGame.Name}' ('{hostGame.Id}') for host game '{protoGame.Name}' ('{protoGame.Id}') from host '{host.HostName}' ('{host.Ip}'): '{e.Message}'");
                }

                if (hostGames.Count == protoGame.HostGames.Count) continue;

                GameVariant newGameVariant;
                Models.Database.Game? game;
                if (newHostGames.Count == protoGame.HostGames.Count)
                {
                    game = await databaseContext.Games.FirstOrDefaultAsync(
                        g => g.Name == protoGame.Name, cancellationToken: token);
                    if (game is null)
                    {
                        var genres = await GetNamedDatabaseObjectsAsync(
                            databaseContext.Genres, cache.Genres, protoGame.Genres, token);
                        var series = await GetNamedDatabaseObjectsAsync(
                            databaseContext.Series, cache.Series, protoGame.Series, token);
                        var tags = await GetNamedDatabaseObjectsAsync(
                            databaseContext.Tags,
                            cache.Tags,
                            protoGame.HostGames.SelectMany(h => h.Tags).Distinct(),
                            token);
                        game = databaseContext.Games.Add(new()
                        {
                            Name = protoGame.Name,
                            SortingName = protoGame.SortingName,
                            Description = protoGame.Description,
                            Genres = genres,
                            Tags = tags,
                            Series = series
                        }).Entity;
                        databaseContext.UserGameInfo.Add(new()
                        {
                            User = user,
                            Favorite = protoGame.HostGames.Any(h => h.UserGameInfo.Any(i => i.Favorite)),
                            Game = game,
                            CompletionStatus = completionStatus
                        });
                    }

                    var features = await GetNamedDatabaseObjectsAsync(
                        databaseContext.Features,
                        cache.Features,
                        protoGame.HostGames.SelectMany(h => h.Features).Distinct(),
                        token);
                    var developers = await GetNamedDatabaseObjectsAsync(
                        databaseContext.Companies,
                        cache.Companies,
                        protoGame.HostGames.SelectMany(h => h.Developers).Distinct(),
                        token);
                    var publishers = await GetNamedDatabaseObjectsAsync(
                        databaseContext.Companies,
                        cache.Companies,
                        protoGame.HostGames.SelectMany(h => h.Publishers).Distinct(),
                        token);
                    var platforms = await GetNamedDatabaseObjectsAsync(
                        databaseContext.Platforms,
                        cache.Platforms,
                        protoGame.HostGames.SelectMany(h => h.Platforms).Distinct(),
                        token);
                    newGameVariant = await databaseContext.GamesVariants.FirstOrDefaultAsync(
                        g => g.Name == protoGame.Name, cancellationToken: token)
                                        ?? databaseContext.GamesVariants.Add(new()
                                        {
                                            Games = [game],
                                            Name = protoGame.Name,
                                            Description = protoGame.Description,
                                            Features = features,
                                            Developers = developers,
                                            Publishers = publishers,
                                            Platforms = platforms,
                                            GameVariantType = defaultGameVariantType

                                        }).Entity;
                }
                else
                {
                    var libraryGameId = hostGames.Select(h => h.Value).First().LibraryGameId;
                    var libraryGame = await databaseContext
                        .LibraryGames
                        .Include(l => l.GameVariant)
                        .ThenInclude(v => v.Games)
                        .Select(l => new { l.Id, l.GameVariant })
                        .FirstAsync(l => l.Id == libraryGameId, token);
                    newGameVariant = libraryGame.GameVariant;
                    game = newGameVariant.Games.First();
                }

                foreach (var (newHostGame, libraryGame, library) in newHostGames)
                {
                    var addedLibraryGame = libraryGame ?? databaseContext.LibraryGames.Add(new()
                    {
                        LibraryId = library.Id,
                        LibraryGameId = newHostGame.LibraryGameId,
                        GameVariant = newGameVariant

                    }).Entity;
                    databaseContext.HostGames.Add(new()
                    {
                        HostId = host.Id,
                        HostGameId = newHostGame.Id.ToGuid().ToString(),
                        LibraryGameId = addedLibraryGame.Id,
                        Installed = newHostGame.Installed,
                        Playing = newHostGame.Playing,
                        Size = newHostGame.Size is 0 ? null : newHostGame.Size,
                        InstallPath = string.Empty,
                        Version = string.Empty
                    });

                    var hostGameId = newHostGame.Id.ToGuid();
                    await databaseContext.SaveChangesAsync(token);
                    mediaDownloadTasks.AddRange(CreateMediaDownloadTasks(host, hostGameId, game.Id, token));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error fetching games from host '{host.HostName}' ('{host.Ip}'): '{ex.Message}'");
        }

        try
        {
            await Task.WhenAll(mediaDownloadTasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while downloading game media");
        }
    }

    private List<Task> CreateMediaDownloadTasks(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token) =>
    [
        mediaFileService.DownloadLogoAsync        (host, hostGameId, gameId, token).AsTask(),
        mediaFileService.DownloadIconAsync        (host, hostGameId, gameId, token).AsTask(),
        mediaFileService.DownloadTrailerAsync     (host, hostGameId, gameId, token).AsTask(),
        mediaFileService.DownloadMicroTrailerAsync(host, hostGameId, gameId, token).AsTask(),
        mediaFileService.DownloadBackgroundAsync  (host, hostGameId, gameId, token),
        mediaFileService.DownloadCoverAsync       (host, hostGameId, gameId, token),
        mediaFileService.DownloadMusicAsync       (host, hostGameId, gameId, token),
    ];

    private async Task<Cache> GetHostCacheAsync(Models.Database.Host host, DatabaseContext databaseContext, CancellationToken token)
    {
        var featuresTask = GetLabelsAsync(host, databaseContext.Features, hostClient.ListFeaturesAsync, token);
        var seriesTask = GetLabelsAsync(host, databaseContext.Series, hostClient.ListSeriesAsync, token);
        var companiesTask = GetLabelsAsync(host, databaseContext.Companies, hostClient.ListCompaniesAsync, token);
        var librariesTask = GetLabelsAsync(host, databaseContext.Companies, hostClient.ListCompaniesAsync, token);

        var cache = await hostClient.GetCacheAsync(host, token);
        await UpdateNamedObjectsAsync(databaseContext.Genres, cache.Genres, token);
        await UpdateNamedObjectsAsync(databaseContext.Tags, cache.Tags, token);
        await UpdateNamedObjectsAsync(databaseContext.Platforms, cache.Platforms, token);

        var features = await featuresTask;
        var series = await seriesTask;
        var companies = await companiesTask;
        var libraries = await librariesTask;

        await databaseContext.SaveChangesAsync(token);
        return new()
        {
            Genres = cache.Genres,
            Tags = cache.Tags,
            Platforms = cache.Platforms,
            Features = features,
            Companies = companies,
            Series = series,
            Libraries = libraries
        };
    }

    private static async Task<IList<Label>> GetLabelsAsync<T>(
        Models.Database.Host host,
        DbSet<T> set,
        Func<Models.Database.Host, CancellationToken, Task<IList<Label>>> func,
        CancellationToken token)
        where T : NamedDatabaseObject, new()
    {
        var labels = await func(host, token);
        await UpdateNamedObjectsAsync(set, labels, token);
        return labels;
    }

    private static async Task UpdateNamedObjectsAsync<T>(
        DbSet<T> set, IList<Label> labels, CancellationToken token) where T : NamedDatabaseObject, new()
    {
        var existingLabels = await set.Select(g => g.Name).ToListAsync(token);
        var newLabels = labels.Where(g => !existingLabels.Contains(g.Name)).Select(g => new T { Name = g.Name });
        set.AddRange(newLabels);
    }

    private static Task<List<T>> GetNamedDatabaseObjectsAsync<T>(
        DbSet<T> set, ICollection<Label> cache, IEnumerable<ByteString> ids, CancellationToken token)
        where T : NamedDatabaseObject
    {
        var genres = ids.Select(g => cache.First(cg => cg.Id == g).Name).ToList();
        return set.Where(g => genres.Contains(g.Name)).ToListAsync(token);
    }

    private static Task<List<T>> GetNamedDatabaseObjectsAsync<T>(
        DbSet<T> set, ICollection<Label> cache, IEnumerable<string> names, CancellationToken token)
        where T : NamedDatabaseObject
    {
        var genres = names.Select(g => cache.First(cg => cg.Name == g).Name).ToList();
        return set.Where(g => genres.Contains(g.Name)).ToListAsync(token);
    }

    public async Task HandleOperationStateChangeAsync(
        HostOperationChangedEventArgs args,
        CancellationToken cancellationToken = default)
    {
        if (!IsGameOperation(args.Type))
        {
            return;
        }

        if (args.State is not OperationState.Finished and not OperationState.Failed)
        {
            return;
        }

        if (args.TargetId == Guid.Empty)
        {
            return;
        }

        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var databaseHostGame = await databaseContext
            .HostGames
            .Include(g => g.Host)
            .Include(g => g.LibraryGame)
            .ThenInclude(l => l.Library)
            .Include(g => g.LibraryGame)
            .ThenInclude(l => l.GameVariant)
            .ThenInclude(v => v.Games)
            .FirstOrDefaultAsync(h => h.Id == args.TargetId, cancellationToken);
        if (databaseHostGame is null)
        {
            return;
        }

        var result = await UpdateHostGameStateAsync(databaseHostGame, cancellationToken);
        if (result)
        {
            await databaseContext.SaveChangesAsync(cancellationToken);
        }

        if (GameUpdated is not null)
        {
            await GameUpdated.Invoke(new GameUpdatedEventArgs(databaseHostGame.LibraryGame.GameVariant.Games.First(), args.Type, args.State));
        }
    }

    public async Task UpdateGameStateAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var game = await databaseContext
                        .Games
                        .Include(g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.HostGames)
                        .ThenInclude(h => h.Host)
                        .Include(g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.Library)
                        .FirstAsync(g => g.Id == gameId, cancellationToken);
        var updateGameResults = await game
             .GameVariants
             .SelectMany(v => v.LibraryGames.SelectMany(g => g.HostGames))
             .SelectAsync(h => UpdateHostGameStateAsync(h, cancellationToken))
             .ToListAsync();
        if (updateGameResults.All(r => !r))
        {
            return;
        }

        await databaseContext.SaveChangesAsync(cancellationToken);

        if (GameUpdated is not null)
        {
            await GameUpdated.Invoke(new GameUpdatedEventArgs(game, null, null));
        }
    }

    public async Task UpdatePlayingGamesAsync(Models.Database.Host host, CancellationToken token)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(token);
        var playingHostGames = await databaseContext
            .HostGames
            .Include(h => h.LibraryGame)
            .ThenInclude(l => l.Library)
            .Include(h => h.LibraryGame)
            .ThenInclude(l => l.GameVariant)
            .ThenInclude(v => v.Games)
            .Where(h => h.HostId == host.Id && h.Playing)
            .ToListAsync(token);

        if (playingHostGames.Count is 0)
        {
            return;
        }

        List<HostGame> changedGames = [];

        foreach (var hostGame in playingHostGames)
        {
            try
            {
                var protoHostGame = await hostClient.GetGameAsync(host, hostGame, token);
                if (protoHostGame is null)
                {
                    continue;
                }

                var wasPlaying = hostGame.Playing;
                hostGame.Installed = protoHostGame.Installed;
                hostGame.Playing = protoHostGame.Playing;
                hostGame.Size = protoHostGame.Size is 0 ? null : protoHostGame.Size;

                if (wasPlaying != hostGame.Playing)
                {
                    changedGames.Add(hostGame);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not retrieve playing state for host game '{HostGameId}' on host '{HostId}' ({HostName}).",
                    hostGame.HostGameId, host.Id, host.HostName);
            }
        }

        await databaseContext.SaveChangesAsync(token);

        if (GameUpdated is null) return;

        foreach (var changed in changedGames)
        {
            await GameUpdated.Invoke(new GameUpdatedEventArgs(changed.LibraryGame.GameVariant.Games.First(), null, null));
        }
    }

    private async Task<bool> UpdateHostGameStateAsync(HostGame databaseHostGame, CancellationToken cancellationToken = default)
    {
        try
        {
            var hostGameIdByteStr = Guid.Parse(databaseHostGame.HostGameId).ToByteString();

            var hostGame = await hostClient.GetGameAsync(databaseHostGame.Host, databaseHostGame, cancellationToken);
            if (hostGame is null)
            {
                return false;
            }

            if (databaseHostGame.Installed == hostGame.Installed
             && databaseHostGame.Playing == hostGame.Playing
             && databaseHostGame.Size == hostGame.Size)
            {
                return false;
            }

            databaseHostGame.Installed = hostGame.Installed;
            databaseHostGame.Playing = hostGame.Playing;
            databaseHostGame.Size = hostGame.Size is 0 ? null : hostGame.Size;
            return true;
        }
        catch (Exception e)
        {
            logger.LogError($"Error updating game state for host game '{databaseHostGame.HostGameId}' on host '{databaseHostGame.Host.DisplayName}' ('{databaseHostGame.Host.Ip}'): '{e.Message}'");
        }

        return false;
    }

    private static bool IsGameOperation(OperationType operationType) => operationType switch
    {
        OperationType.StartGame => true,
        OperationType.MonitorGame => true,
        OperationType.StopGame => true,
        OperationType.InstallGame => true,
        OperationType.UninstallGame => true,
        OperationType.UpdateGame => true,
        OperationType.RemoveGame => true,
        OperationType.RepairGame => true,
        OperationType.MoveGame => true,
        _ => false
    };
}
