using Server.Models.Database;

namespace Server.Models.Events;

public sealed record GameUpdatedEventArgs(
    Game Game,
    OperationType? Type,
    OperationState? State);
