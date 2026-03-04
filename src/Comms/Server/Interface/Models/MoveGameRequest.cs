namespace Comms.Server.Interface.Models;

public class MoveGameRequest : GameInstanceRequest
{
    public string Path { get; set; } = string.Empty;
}