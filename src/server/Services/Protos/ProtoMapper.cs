using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Models.Database;
using Common.Utility.Extensions;
using Label = Satelight.Protos.Core.Label;

namespace Server.Services.Protos;

internal static class ProtoMapper
{
    public static Op CreateOp(Operation op) => new()
    {
        Id   = op.Id.ToByteString(),
        Info = CreateOpInfo(op)
    };

    public static OpInfo CreateOpInfo(Operation op) => new()
    {
        State    = ConvertOpState(op.State),
        Progress = op.Progress,
        Host     = op.Host.DnsName,
        Type     = ConvertRequestType(op.Type),
        TargetId = ByteString.CopyFrom(op.TargetId.ToByteArray())
    };

    private static OpType ConvertRequestType(OperationType operationType) => operationType switch
    {
        OperationType.StartGame     => OpType.StartGame,
        OperationType.MonitorGame   => OpType.MonitorGame,
        OperationType.StopGame      => OpType.StopGame,
        OperationType.InstallGame   => OpType.InstallGame,
        OperationType.UninstallGame => OpType.UninstallGame,
        OperationType.UpdateGame    => OpType.UpdateGame,
        OperationType.RemoveGame    => OpType.RemoveGame,
        OperationType.RepairGame    => OpType.RepairGame,
        OperationType.MoveGame      => OpType.MoveGame,
        OperationType.SleepHost     => OpType.SleepHost,
        OperationType.PowerOffHost  => OpType.PowerOffHost,
        OperationType.PowerOnHost   => OpType.PowerOnHost,
        OperationType.UpdateHost    => OpType.UpdateHost,
        _                           => OpType.None
    };

    public static OpState ConvertOpState(OperationState operationState) => operationState switch
    {
        OperationState.Queued   => OpState.Queued,
        OperationState.Running  => OpState.Running,
        OperationState.Paused   => OpState.Paused,
        OperationState.Finished => OpState.Finished,
        OperationState.Failed   => OpState.Failed,
        _                       => OpState.Unknown
    };

    public static OperationState ConvertOpState(OpState opState) => opState switch
    {
        OpState.Queued   => OperationState.Queued,
        OpState.Running  => OperationState.Running,
        OpState.Paused   => OperationState.Paused,
        OpState.Finished => OperationState.Finished,
        OpState.Failed   => OperationState.Failed,
        _                => OperationState.Unknown
    };

    public static async Task AddLabelsAsync<T>(
        RepeatedField<Label> protoLabels, DbSet<T> labels, CancellationToken token)
        where T : NamedDatabaseObject
    {
        var labelList = await labels.ToListAsync(token);
        protoLabels.AddRange(labelList.Select(ConvertLabel));
    }
    public static void AddIds<T>(RepeatedField<ByteString> ids, ICollection<T> objects)
        where T : DatabaseObject
        => ids.AddRange(objects.Select(o => o.Id.ToByteString()));

    public static Label ConvertLabel(NamedDatabaseObject gameLabel) => new()
    {
        Id   = gameLabel.Id.ToByteString(),
        Name = gameLabel.Name
    };
}