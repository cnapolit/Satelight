using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class GetFiltersResponse : SatelightResponse
{
    public IList<Filter> Filters { get; set; } = [];
}