namespace Comms.Host.Interface.Models;

public class RemoveGameRequest : HostGameRequest
{
    public bool IgnoreOnScan { get; set; }
}