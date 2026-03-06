namespace Comms.Common.Interface.Models;

public enum OpState
{
    Unknown = 0,
    Queued = 1,
    Paused = 2,
    Running = 3,
    Finished = 4,
    Failed = 5
}