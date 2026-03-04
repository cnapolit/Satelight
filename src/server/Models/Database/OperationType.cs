namespace Server.Models.Database;

public enum OperationType 
{
    None,
    StartGame,
    MonitorGame,
    StopGame,
    InstallGame,
    UninstallGame,
    UpdateGame,
    RemoveGame,
    RepairGame,
    MoveGame,
    SleepHost,
    PowerOffHost,
    PowerOnHost,
    UpdateHost
}