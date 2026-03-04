using System.Collections.Concurrent;
using Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;
using Server.Models.Database;
using Server.Models.Events;
using Server.Services.Protos;

namespace Server.Services.Host;

public class HostOperationPollingService(
    ILogger<HostOperationPollingService> logger,
    IDbContextFactory<DatabaseContext> databaseContextFactory,
    ChannelManager channelManager,
    HostClient hostClient) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private readonly ConcurrentDictionary<Guid, OperationState> _lastOpStates = new();

    public event Func<HostOperationChangedEventArgs, Task>? OperationChanged;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(PollInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollActiveHostsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to poll host operations.");
            }

            if (!await timer.WaitForNextTickAsync(stoppingToken))
            {
                break;
            }
        }
    }

    private async Task PollActiveHostsAsync(CancellationToken token)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(token);
        var activeHostIds = await databaseContext
            .Operations
            .AsNoTracking()
            .Where(o => o.State == OperationState.Queued
                     || o.State == OperationState.Running
                     || o.State == OperationState.Paused)
            .Select(o => o.HostId)
            .Distinct()
            .ToListAsync(token);
        if (activeHostIds.Count == 0)
        {
            return;
        }

        var hosts = await databaseContext
            .Hosts
            .AsNoTracking()
            .Where(h => activeHostIds.Contains(h.Id))
            .ToListAsync(token);

        foreach (var host in hosts)
        {
            await PollHostOperationsAsync(host, token);
        }
    }

    private async Task PollHostOperationsAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Ops.OpsClient opsClient = new(channel);
        ListOpsReply reply;
        try
        {
            reply = await opsClient.ListOpsAsync(new(), cancellationToken: token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not retrieve operations for host '{HostId}' ({HostName}).", host.Id, host.HostName);
            return;
        }

        await UpsertHostOperationsAsync(host.Id, reply.Ops, token);
    }

    private async Task UpsertHostOperationsAsync(Guid hostId, IEnumerable<Op> hostOps, CancellationToken token)
    {
        var ops = hostOps.ToList();
        if (ops.Count == 0)
        {
            return;
        }

        List<Guid> hostOpIds = [];
        foreach (var op in ops)
        {
            if (TryGetGuid(op.Id, out var hostOpId))
            {
                hostOpIds.Add(hostOpId);
            }
        }
        if (hostOpIds.Count == 0)
        {
            return;
        }

        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(token);
        var existingOps = await databaseContext
            .Operations
            .Where(o => o.HostId == hostId && hostOpIds.Contains(o.HostOpId))
            .ToDictionaryAsync(o => o.HostOpId, token);

        List<HostOperationChangedEventArgs> changedOps = [];

        foreach (var op in ops)
        {
            if (!TryGetGuid(op.Id, out var hostOpId))
            {
                continue;
            }

            // Update logic to find database version of host target id
            if (!existingOps.TryGetValue(hostOpId, out var databaseOperation))
            {
                continue;
                databaseOperation = new Operation
                {
                    HostId = hostId,
                    HostOpId = hostOpId,
                    Type = ConvertOpType(op.Info.Type),
                    TargetId = TryGetGuid(op.Info.TargetId, out var targetId) ? targetId : Guid.Empty,
                };
                databaseContext.Operations.Add(databaseOperation);
            }

            var previousState = databaseOperation.State;
            if (previousState == OperationState.Unknown
                && _lastOpStates.TryGetValue(hostOpId, out var lastState))
            {
                previousState = lastState;
            }

            var nextState = ProtoMapper.ConvertOpState(op.Info.State);
            databaseOperation.State = nextState;
            databaseOperation.Progress = (short)Math.Clamp(op.Info.Progress, short.MinValue, short.MaxValue);

            _lastOpStates[hostOpId] = nextState;

            if (OpStateChanged(previousState, nextState))
            {
                changedOps.Add(new HostOperationChangedEventArgs(
                    hostOpId,
                    databaseOperation.TargetId,
                    databaseOperation.Type,
                    nextState));
            }
        }

        await databaseContext.SaveChangesAsync(token);

        if (changedOps.Count == 0) return;

        foreach (var changed in changedOps)
        {
            await hostClient.HandleOperationStateChangeAsync(changed, token);
        }

        if (OperationChanged is null) return;

        await Task.WhenAll(changedOps.Select(OperationChanged.Invoke).ToList());
    }

    private static OperationType ConvertOpType(OpType opType) => opType switch
    {
        OpType.StartGame => OperationType.StartGame,
        OpType.MonitorGame => OperationType.MonitorGame,
        OpType.StopGame => OperationType.StopGame,
        OpType.InstallGame => OperationType.InstallGame,
        OpType.UninstallGame => OperationType.UninstallGame,
        OpType.UpdateGame => OperationType.UpdateGame,
        OpType.RemoveGame => OperationType.RemoveGame,
        OpType.RepairGame => OperationType.RepairGame,
        OpType.MoveGame => OperationType.MoveGame,
        OpType.SleepHost => OperationType.SleepHost,
        OpType.PowerOffHost => OperationType.PowerOffHost,
        OpType.PowerOnHost => OperationType.PowerOnHost,
        OpType.UpdateHost => OperationType.UpdateHost,
        _ => OperationType.None
    };

    private static bool TryGetGuid(Google.Protobuf.ByteString value, out Guid guid)
    {
        try
        {
            guid = value.ToGuid();
            return true;
        }
        catch
        {
            guid = Guid.Empty;
            return false;
        }
    }

    private static bool OpStateChanged(OperationState previousState, OperationState nextState)
        => nextState != OperationState.Unknown && nextState != previousState;
}
