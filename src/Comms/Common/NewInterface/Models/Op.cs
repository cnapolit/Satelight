using System;

namespace Comms.Common.Interface.Models;

public class Op : OpInfo
{
    private Guid? _id;
    public Guid Id { get => _id ?? Guid.Empty; set => _id ??= value; }
    public RequestType Type { get; init; }
    public OpState State { get; set; }
    public Guid GameId { get; init; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}