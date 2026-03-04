namespace Server.Models.Database;

public enum OperationState
{
    Unknown,
    Queued,
    Running,
    Paused,
    Finished,
    Failed
}