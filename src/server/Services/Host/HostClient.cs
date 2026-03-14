using System.Net;
using System.Net.NetworkInformation;
using Common.Utility.Classes;
using Common.Utility.Extensions;
using Common.Utility.Functions;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Satelight.Protos.Host;
using Server.Database;
using Server.Models.Database;
using Server.Models.Events;
using Server.Models.Host;
using Server.Services.Protos;
using HostGame = Server.Models.Database.HostGame;
using Series = Satelight.Protos.Core.Series;

namespace Server.Services.Host;

public class HostClient(
    ILogger<HostClient> logger,
    IDbContextFactory<DatabaseContext> databaseContextFactory,
    ChannelManager channelManager,
    MediaFileService mediaFileService)
{
    public event Func<GameUpdatedEventArgs, Task>? GameUpdated;

    public async Task GetGamesAsync(
        Action<string, int, int> progressCallback, Action<int, int> stepProgressCallback, CancellationToken token)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(token);
        var hosts = await databaseContext.Hosts.ToListAsync(token);
        // hosts = await hosts.WhereAsync(HostIsAvailableAsync, token).ToListAsync();
        var progress = 0;
        var total = hosts.Count + 1;
        progressCallback("Initializing host caches", progress++, total);
        var stepProgress = 0;
        stepProgressCallback(stepProgress++, hosts.Count);
        var user = await databaseContext.Users.FirstAsync(token);
        var completionStatus = await databaseContext.CompletionStatuses.FirstAsync(token);

        Dictionary<Guid, Cache> hostCaches = [];
        await hosts.ForEachAsync(async h =>
        {
            var channel = channelManager.GetChannel(h);
            Satelight.Protos.Core.Database.DatabaseClient databaseClient = new(channel);
            var cache = await databaseClient.GetCacheAsync(new(), cancellationToken: token);

            await UpdateNamedObjectsAsync(databaseContext.Genres, cache.Genres, token);
            await UpdateNamedObjectsAsync(databaseContext.Tags, cache.Tags, token);
            await UpdateNamedObjectsAsync(databaseContext.Platforms, cache.Platforms, token);

            Features.FeaturesClient featuresClient = new(channel);
            var featuresReply = await featuresClient.ListAsync(new(), cancellationToken: token);
            await UpdateNamedObjectsAsync(databaseContext.Features, featuresReply.Labels, token);

            Series.SeriesClient seriesClient = new(channel);
            var seriesReply = await seriesClient.ListAsync(new(), cancellationToken: token);
            await UpdateNamedObjectsAsync(databaseContext.Series, seriesReply.Labels, token);

            Companies.CompaniesClient companiesClient = new(channel);
            var companiesReply = await companiesClient.ListAsync(new(), cancellationToken: token);
            await UpdateNamedObjectsAsync(databaseContext.Companies, companiesReply.Labels, token);

            Libraries.LibrariesClient librariesClient = new(channel);
            var librariesReply = await librariesClient.ListAsync(new(), cancellationToken: token);
            await UpdateNamedObjectsAsync(databaseContext.Libraries, librariesReply.Labels, token);
            await databaseContext.SaveChangesAsync(token);
            stepProgressCallback(stepProgress++, hosts.Count);
            hostCaches[h.Id] = new()
            {
                Genres    = cache.Genres,
                Tags      = cache.Tags,
                Platforms = cache.Platforms,
                Features  = featuresReply.Labels,
                Companies = companiesReply.Labels,
                Series    = seriesReply.Labels,
                Libraries = librariesReply.Labels
            };
        });

        var defaultGameVariantType = await databaseContext.GameVariantTypes.FirstAsync(
            v => v.GameVariantFlags.HasFlag(GameVariantFlags.Default), token);
        foreach (var host in hosts) try
        {
            var cache = hostCaches[host.Id];
            var channel = channelManager.GetChannel(host);
            Games.GamesClient gamesClient = new(channel);
            var gamesCountReply = await gamesClient.CountAsync(new(), cancellationToken: token);
            progressCallback($"Retrieving games from host '{host.DisplayName}'", progress++, total);
            stepProgress = 0;
            var gameStream = gamesClient.Stream(new(), cancellationToken: token);
            await foreach (var protoGame in gameStream.ResponseStream.ReadAllAsync(token))
            {
                stepProgressCallback(stepProgress++, gamesCountReply.Count);
                Dictionary<Guid, HostGame> hostGames = [];
                List<(Satelight.Protos.Host.HostGame, LibraryGame?, Library)> newHostGames = [];
                foreach (var hostGame in protoGame.HostGames) try
                {
                    var hostGameId = hostGame.Id.ToGuid().ToString();
                    var databaseHostGame = await databaseContext
                        .HostGames
                        .Include    (h => h.LibraryGame)
                        .ThenInclude(l => l.GameVariant)
                        .ThenInclude(v => v.Games)
                        .FirstOrDefaultAsync(
                            h => h.HostId     == host.Id 
                              && h.HostGameId == hostGameId,
                            cancellationToken: token);
                    if (databaseHostGame != null)
                    {
                        hostGames[databaseHostGame.Id] = databaseHostGame;
                        var game = databaseHostGame.LibraryGame.GameVariant.Games.First();
                        await UpdateHostGameAsync(host, hostGame.Id.ToGuid(), game.Id, token);
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
                        library = await databaseContext.Libraries.FirstAsync(
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
                if (newHostGames.Count == protoGame.HostGames.Count)
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
                    var game = await databaseContext.Games.FirstOrDefaultAsync(
                        g => g.Name == protoGame.Name, cancellationToken: token);
                    if (game is null)
                    {
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
                        Games       = [game],
                        Name        = protoGame.Name,
                        Description = protoGame.Description,
                        Features    = features,
                        Developers  = developers,
                        Publishers  = publishers,
                        Platforms   = platforms,
                        GameVariantType = defaultGameVariantType

                    }).Entity;
                }
                else
                {
                    var libraryGameId = hostGames.Select(h => h.Value).First().LibraryGameId;
                    var libraryGame = await databaseContext
                        .LibraryGames
                        .Include    (l => l.GameVariant)
                        .ThenInclude(v => v.Games)
                        .Select     (l => new { l.Id, l.GameVariant })
                        .FirstAsync (l => l.Id == libraryGameId, token);
                    newGameVariant = libraryGame.GameVariant;
                }

                foreach (var (newHostGame, libraryGame, library) in newHostGames)
                {
                    var addedLibraryGame = libraryGame ?? databaseContext.LibraryGames.Add(new()
                    {
                        LibraryId     = library.Id,
                        Library       = library,
                        LibraryGameId = newHostGame.LibraryGameId,
                        GameVariant   = newGameVariant

                    }).Entity;
                    databaseContext.HostGames.Add(new()
                    {
                        HostId        = host.Id,
                        HostGameId    = newHostGame.Id.ToGuid().ToString(),
                        LibraryGameId = addedLibraryGame.LibraryId,
                        Host          = host,
                        LibraryGame   = addedLibraryGame,
                        Installed     = newHostGame.Installed,
                        Playing       = newHostGame.Playing,
                        Size          = newHostGame.Size is 0 ? null : newHostGame.Size,
                        InstallPath   = string.Empty,
                        Version       = string.Empty
                    });
                    
                    await databaseContext.SaveChangesAsync(token);

                    var hostGameId = newHostGame.Id.ToGuid();
                    var gameId     = newGameVariant.Games.First().Id;
                    await UpdateHostGameAsync(host, hostGameId, gameId, token);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error fetching games from host '{host.HostName}' ('{host.Ip}'): '{ex.Message}'");
        }
    }

    private Task UpdateHostGameAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => ValueTaskAwaiter
            .WhenAll(mediaFileService.DownloadLogoAsync        (host, hostGameId, gameId, token),
                     mediaFileService.DownloadIconAsync        (host, hostGameId, gameId, token),
                     mediaFileService.DownloadTrailerAsync     (host, hostGameId, gameId, token),
                     mediaFileService.DownloadMicroTrailerAsync(host, hostGameId, gameId, token))
          .WithAsync(mediaFileService.DownloadBackgroundAsync  (host, hostGameId, gameId, token),
                     mediaFileService.DownloadCoverAsync       (host, hostGameId, gameId, token),
                     mediaFileService.DownloadMusicAsync       (host, hostGameId, gameId, token));

    private static async ValueTask<bool> HostIsAvailableAsync(Models.Database.Host host, CancellationToken token)
        => IPAddress.TryParse(host.Ip, out var remoteIp)
        && NetworkInterface.GetAllNetworkInterfaces()
          .Any(n => n.OperationalStatus is OperationalStatus.Up 
                 && n.GetIPProperties().UnicastAddresses.Any(u => IsInSameSubnet(u, remoteIp)))
        && await Async.TryAsync(GetIpStatusAsync, host.Ip, token) is IPStatus.Success;

    private static bool IsInSameSubnet(UnicastIPAddressInformation addressInfo, IPAddress ipAddress)
    {
        if (addressInfo.Address.AddressFamily != ipAddress.AddressFamily)
        {
            return false;
        }

        var a1 =  addressInfo.Address.GetAddressBytes();
        var a2 =            ipAddress.GetAddressBytes();
        if (a1.Length != a2.Length)
        {
            return false;
        }

        var m = addressInfo.IPv4Mask.GetAddressBytes();
        return a1.Length == m.Length
            && !a1.Where((t, i) => (t & m[i]) != (a2[i] & m[i])).Any();
    }

    private static async Task UpdateNamedObjectsAsync<T>(
        DbSet<T> set, RepeatedField<Label> labels, CancellationToken token) where T : NamedDatabaseObject, new()
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

    private static async Task<IPStatus> GetIpStatusAsync(string ip, CancellationToken token)
    {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, TimeSpan.FromSeconds(1), cancellationToken: token);
            return reply.Status;
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
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.HostGames)
                        .ThenInclude(h => h.Host)
                        .Include    (g => g.GameVariants)
                        .ThenInclude(g => g.LibraryGames)
                        .ThenInclude(g => g.Library)
                        .FirstAsync (g => g.Id == gameId, cancellationToken);
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

        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        List<HostGame> changedGames = [];

        foreach (var hostGame in playingHostGames)
        {
            try
            {
                var hostGameIdByteStr = Guid.Parse(hostGame.HostGameId).ToByteString();
                var reply = await gamesClient.GetAsync(new()
                {
                    GameId        = hostGameIdByteStr,
                    Library       = hostGame.LibraryGame.Library.Name,
                    LibraryGameId = hostGame.LibraryGame.Id.ToString()
                }, cancellationToken: token);

                var protoHostGame = reply.Game?.HostGames?.FirstOrDefault(h => h.Id == hostGameIdByteStr);
                if (protoHostGame is null)
                {
                    continue;
                }

                var wasPlaying    = hostGame.Playing;
                hostGame.Installed = protoHostGame.Installed;
                hostGame.Playing   = protoHostGame.Playing;
                hostGame.Size      = protoHostGame.Size is 0 ? null : protoHostGame.Size;

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
            var channel = channelManager.GetChannel(databaseHostGame.Host);
            Games.GamesClient gamesClient = new(channel);
            var hostGameIdByteStr = Guid.Parse(databaseHostGame.HostGameId).ToByteString();
            var getGameReply = await gamesClient.GetAsync(new()
            {
                GameId = hostGameIdByteStr,
                Library = databaseHostGame.LibraryGame.Library.Name,
                LibraryGameId = databaseHostGame.LibraryGame.Id.ToString()
            }, cancellationToken: cancellationToken);

            var hostGame = getGameReply.Game?.HostGames?.FirstOrDefault(h => h.Id == hostGameIdByteStr);
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
    
    public Task<Op> InstallAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.InstallGame, libraryId, libraryGameId, hostId, InstallAsync, cancellationToken);

    private static async Task<Op> InstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.InstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> UninstallAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.UninstallGame, libraryId, libraryGameId, hostId, UninstallAsync, cancellationToken);

    private static async Task<Op> UninstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.UninstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> RepairAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.RepairGame, libraryId, libraryGameId, hostId, RepairAsync, cancellationToken);

    private static async Task<Op> RepairAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.RepairAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StartAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.StartGame, libraryId, libraryGameId, hostId, StartAsync, cancellationToken);

    private static async Task<Op> StartAsync(
        Games.GamesClient gamesClient, 
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.StartAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StopAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.StopGame, libraryId, libraryGameId, hostId, StopAsync, cancellationToken);

    private static async Task<Op> StopAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.StopAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    private async Task<Op> TriggerActionAsync(
        OperationType operationType,
        Guid libraryId,
        Guid libraryGameId,
        Guid hostId,
        Func<Games.GamesClient, GameIdentifier, CancellationToken, Task<Op>> action,
        CancellationToken cancellationToken)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var game = await databaseContext
                        .LibraryGames
                        .AsNoTracking()
                        .Include(g => g.HostGames)
                        .Include(l => l.Library)
                        .FirstOrDefaultAsync(l => l.LibraryId == libraryId && l.Id == libraryGameId, cancellationToken)
                ?? throw new($"Library Game '{libraryId}' does not exist");

        var host = await databaseContext
                        .Hosts
                        .AsNoTracking()
                        .Include(h => h.Libraries)
                        .Include(h => h.Games)
                        .FirstOrDefaultAsync(d => d.Id == hostId, cancellationToken)
                ?? throw new($"Host '{hostId}' does not exist");
        if (!host.Libraries.Contains(game.Library))
        {
            // throw new($"Host '{host.HostName}' does not have access to Library '{game.Library.Name}'");
        }

        var hostGame = host.Games.FirstOrDefault(g => g.LibraryGameId == game.Id);
        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        // TODO: Check if the host is reachable
        if (hostGame is null)
        {
            throw new($"Host does not have game '{game.LibraryGameId}' for library '{game.Library.Name}''");
            Satelight.Protos.Core.Database.DatabaseClient databaseClient = new(channel);
            _ = await databaseClient.UpdateAsync(new(), cancellationToken: cancellationToken);
            // TODO: wait for op to finish
            var newGame = await gamesClient.GetAsync(new(), cancellationToken: cancellationToken);
            var hostGames = newGame.Game.HostGames.Select(h => ConvertToHostGame(h, game)).ToList();
            databaseContext.HostGames.AddRange(hostGames);
            await databaseContext.SaveChangesAsync(cancellationToken);
            hostGame = hostGames.First();
        }

        // Hardcoding as guid but this will not always be true when other platforms are supported
        var op = await action(gamesClient, new() { Id = Guid.Parse(hostGame.HostGameId).ToByteString() }, cancellationToken);
        databaseContext.Operations.Add(new()
        {
            HostOpId = op.Id.ToGuid(),
            HostId = host.Id,
            TargetId = hostGame.Id,
            Type = operationType,
            State = ProtoMapper.ConvertOpState(op.Info.State),
            Progress = (short)Math.Clamp(op.Info.Progress, short.MinValue, short.MaxValue)
        });
        await databaseContext.SaveChangesAsync(cancellationToken);
        return op;
    }

    private static HostGame ConvertToHostGame(Satelight.Protos.Host.HostGame hostGame, LibraryGame libraryGame) => new()
    {
        HostGameId = hostGame.Id.ToGuid().ToString(),
        LibraryGame = libraryGame,
        Installed   = hostGame.Installed,
        Playing     = hostGame.Playing,
        Size = hostGame.Size is 0 ? null : hostGame.Size,
    };

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
