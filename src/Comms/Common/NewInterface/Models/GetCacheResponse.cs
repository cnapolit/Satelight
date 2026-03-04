using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class GetCacheResponse : SatelightResponse
{
    public IList<Filter> Filters { get; set; } = [];
    public IList<Label> Genres { get; set; } = [];
    public IList<Label> Platforms { get; set; } = [];
    public IList<Label> CompletionStatuses { get; set; } = [];
    public IList<Label> Tags { get; set; } = [];
    public IList<Label> Sources { get; set; } = [];
}