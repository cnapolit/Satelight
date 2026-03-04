namespace Server.Models.UserInterface;

public enum GameStatus
{
    Unknown,
    Installed,
    NotInstalled,
    Installing,
    Uninstalling,
    Starting,
    Running,
    Stopping
}