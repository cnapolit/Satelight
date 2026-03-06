namespace Comms.Common.Interface.Models;

public class GetOpResponse : SatelightResponse
{
    public required Op Op { get; init; }
}