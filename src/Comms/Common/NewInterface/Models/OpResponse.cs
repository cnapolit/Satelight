
namespace Comms.Common.Interface.Models;

public abstract class OpResponse : SatelightResponse
{
    public Op Op { get; set; } = new();

    internal OpResponse() { }
}