namespace Service.Models;

public sealed class HostAdvertisementOptions
{
    public string InstanceName { get; set; } = Environment.MachineName;
    public int Port { get; set; }
    public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();
}
