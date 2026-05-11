using System.Net;

namespace Server.Models.Events;

public sealed record HostDiscoveredEventArgs(
    string InstanceName,
    string HostName,
    IReadOnlyList<IPAddress> Addresses,
    int Port,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record HostLostEventArgs(string InstanceName);
