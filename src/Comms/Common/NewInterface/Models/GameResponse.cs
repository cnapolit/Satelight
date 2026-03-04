namespace Comms.Common.Interface.Models;

public class GameResponse : SatelightResponse
{
    public Game Game { get; set; }

    internal GameResponse() { }
}