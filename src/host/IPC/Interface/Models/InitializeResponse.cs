namespace Comms.Common.Interface.Models;

public class InitializeResponse : SatelightResponse
{
    public required string Path { get; set; }
    public required short  Port { get; set; }
}