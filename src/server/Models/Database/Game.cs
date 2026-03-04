using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

public class Game : NamedDatabaseObject
{
    [StringLength(500)]
    public string SortingName { get; set; } = string.Empty;

    [StringLength(8000)]
    public string Description { get; set; } = string.Empty;

    public ICollection<GameVariant> GameVariants { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    public ICollection<Series> Series { get; set; } = [];
    public ICollection<UserGameInfo> UserGameInfo { get; set; } = [];
    public ICollection<WishListGame> WishListEntries { get; set; } = [];
}