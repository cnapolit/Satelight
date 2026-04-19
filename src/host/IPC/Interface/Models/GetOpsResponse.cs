namespace Comms.Common.Interface.Models;

public class GetOpsResponse : SatelightResponse
{
    public IList<Op> Ops { get; set; } = [];
}