namespace Comms.Common.Interface.Models;

public class GameInstance
{
    public string Id                     { get; set; } = string.Empty;
    public string Name                   { get; set; } = string.Empty;
    public bool            Primary       { get; set; }
    public bool            LaunchTarget  { get; set; }
    public bool            Installed     { get; set; }
    public bool            Playing       { get; set; }
    public bool            Favorite      { get; set; }
    public long?           Size          { get; set; }
    public long?           ReleaseDate   { get; set; }
    public string?         Description   { get; set; }
    public string?         Source        { get; set; }
    public string?         LibraryGameId { get; set; }
    public string?         LibraryId     { get; set; }
    public IList<string>   PlayActions   { get; set; } = [];
    public IList<string>   Platforms     { get; set; } = [];
    public IList<string>   Developers    { get; set; } = [];
    public IList<string>   Publishers    { get; set; } = [];
    public IList<string>   Features      { get; set; } = [];
}