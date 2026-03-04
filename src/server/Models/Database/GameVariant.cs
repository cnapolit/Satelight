using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

public class GameVariant : NamedDatabaseObject
{
    [StringLength(500)]
    public string SubName { get; set; } = string.Empty;

    [StringLength(8000)]
    public string Description { get; set; } = string.Empty;

    [MinLength(1)]
    // only collections will include multiple
    public ICollection<Game>        Games           { get; set; } = [];
    public GameVariantType          GameVariantType { get; set; }
    public ICollection<LibraryGame> LibraryGames    { get; set; } = [];
    public ICollection<Feature>     Features        { get; set; } = [];
    public ICollection<Company>     Developers      { get; set; } = [];
    public ICollection<Company>     Publishers      { get; set; } = [];
    public ICollection<Platform>    Platforms       { get; set; } = [];
}