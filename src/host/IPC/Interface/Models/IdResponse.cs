using System;
using Comms.Common.Interface.Models;

namespace Comms.Host.Interface.Models;

public class IdResponse : SatelightResponse
{
    public Guid Id { get; set; }
}