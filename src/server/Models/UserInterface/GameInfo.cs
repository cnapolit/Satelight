namespace Server.Models.UserInterface;

public class GameInfo
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string? TrailerUrl { get; set; }
    public List<GameVersion> Versions { get; set; }
}