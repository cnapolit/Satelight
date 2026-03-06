namespace Comms.Host.Interface.Models;

public class MoveGameRequest : HostGameRequest
{
    public string Path { get; set; } = string.Empty;
}