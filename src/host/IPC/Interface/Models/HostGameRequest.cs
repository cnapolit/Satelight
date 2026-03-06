using System;
using Comms.Common.Interface.Models;

namespace Comms.Host.Interface.Models;

public class HostGameRequest : GameRequest
{
    public Guid PluginId { get; set; }
    public string PluginGameId { get; set; } = string.Empty;
}