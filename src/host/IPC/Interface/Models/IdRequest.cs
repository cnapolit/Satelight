using System;

namespace Comms.Common.Interface.Models;

public class IdRequest : SatelightRequest
{
    public Guid Id { get; set; }
}