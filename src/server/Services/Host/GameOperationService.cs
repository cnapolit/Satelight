using Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Satelight.Protos.Host;
using Server.Database;
using Server.Models.Database;
using Server.Services.Protos;
using HostGame = Server.Models.Database.HostGame;

namespace Server.Services.Host;

public class GameOperationService(
    ILogger<GameOperationService> logger,
    IDbContextFactory<DatabaseContext> databaseContextFactory,
    HostClient hostClient)
{
    public Task<Op> InstallAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.InstallGame, libraryId, libraryGameId, hostId, hostClient.InstallAsync, cancellationToken);

    private static async Task<Op> InstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.InstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> UninstallAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.UninstallGame, libraryId, libraryGameId, hostId, hostClient.UninstallAsync, cancellationToken);

    private static async Task<Op> UninstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.UninstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> RepairAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.RepairGame, libraryId, libraryGameId, hostId, hostClient.RepairAsync, cancellationToken);

    private static async Task<Op> RepairAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.RepairAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StartAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.StartGame, libraryId, libraryGameId, hostId, hostClient.StartAsync, cancellationToken);

    private static async Task<Op> StartAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.StartAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StopAsync(Guid libraryId, Guid libraryGameId, Guid hostId, CancellationToken cancellationToken)
        => TriggerActionAsync(OperationType.StopGame, libraryId, libraryGameId, hostId, hostClient.StopAsync, cancellationToken);

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
        Func<Models.Database.Host, string, CancellationToken, Task<Op>> action,
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
        // TODO: Check if the host is reachable
        if (hostGame is null)
        {
            throw new($"Host does not have game '{game.LibraryGameId}' for library '{game.Library.Name}''");
        }

        // Hardcoding as guid but this will not always be true when other platforms are supported
        var op = await action(host, hostGame.HostGameId, cancellationToken);
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
        Installed = hostGame.Installed,
        Playing = hostGame.Playing,
        Size = hostGame.Size is 0 ? null : hostGame.Size,
    };
}
