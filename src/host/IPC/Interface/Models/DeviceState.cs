using System;
using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class DeviceState
{
    public Guid DeviceId { get; set; }
    public IList<GameInstance> Instances { get; set; } = [];

}