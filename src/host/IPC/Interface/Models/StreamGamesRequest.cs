using System;
using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class StreamGamesRequest : SatelightRequest
{
    public IList<Guid> PluginIds { get; set; } = [];
}