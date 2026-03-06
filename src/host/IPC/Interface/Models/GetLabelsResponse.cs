using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class GetLabelsResponse : SatelightResponse
{
    public IList<Label> Items { get; set; } = [];
}