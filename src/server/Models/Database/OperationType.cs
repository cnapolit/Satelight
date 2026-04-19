namespace Server.Models.Database;

public enum OperationType 
{
    None,
    StartGame,
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