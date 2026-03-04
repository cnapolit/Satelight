namespace Comms.Common.Interface.Models;

public enum RequestType
{
    None          = 0,
    Init          = 1,
    GetGame       = 2,
    StreamGames   = 3,
    GetCache      = 4,
    GetFilters    = 5,
    SetFilter     = 6,
    GetTags       = 7,
    GetPlatforms  = 8,
    GetGenres     = 9,
    GetLibraries  = 10,
    GetGameIds    = 11,
    GetOp         = 12,
    GetOps        = 13,
    StartGame     = 14,
    MonitorGame   = 15,
    StopGame      = 16,
    InstallGame   = 17,
    UninstallGame = 18,
    UpdateGame    = 19,
    RemoveGame    = 20,
    RepairGame    = 21,
    MoveGame      = 22,
    UpdateLibrary = 23,
}