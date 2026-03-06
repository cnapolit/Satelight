using System.Collections.Generic;
using Comms.Common.Interface.Models;

namespace Comms.Host.Interface.Models;

public class ListFilesResponse : SatelightResponse
{
    public ICollection<string> Files { get; set; } = [];
}