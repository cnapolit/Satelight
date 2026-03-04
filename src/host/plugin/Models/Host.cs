using System;
using System.Collections.Generic;

namespace HostPlugin.Models;

public class Host
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string IpAddress { get; set; }
    public string MacAddress { get; init; }
    public string Name { get; init; }
    public int Port { get; set; } = 5156;
    public OperatingSystem OperatingSystem { get; set; }
    public Architecture Architecture { get; set; }
    public List<Guid> Plugins { get; set; } = [];
}