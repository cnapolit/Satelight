namespace Comms.Common.Interface.Models;

public abstract class GameRequest : SatelightRequest
{
    public string Id { get; set; }

    internal GameRequest() { }
}