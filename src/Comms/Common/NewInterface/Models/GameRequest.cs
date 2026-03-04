using System;

namespace Comms.Common.Interface.Models;

public abstract class GameRequest : SatelightRequest
{
    public Guid Id { get; set; }

    internal GameRequest() { }
}