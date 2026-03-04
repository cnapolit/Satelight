using System;

namespace Comms.Common.Interface.Models;

public class OpInfo
{
    public RequestType Type        { get; init; }
    public OpState     State       { get; set; }
    public Guid        GameId      { get; init; }
    public DateTime    LastUpdated { get; set; } = DateTime.UtcNow;
}