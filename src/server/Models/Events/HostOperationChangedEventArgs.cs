using Server.Models.Database;

namespace Server.Models.Events;

public sealed record HostOperationChangedEventArgs(
    Guid HostOpId,
    Guid TargetId,
    OperationType Type,
    OperationState State);
