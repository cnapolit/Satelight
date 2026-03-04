namespace Server.Models.Database;

public class Company : NamedDatabaseObject
{
    public ICollection<GameVariant> PublishedGames { get; set; }
    public ICollection<GameVariant> DevelopedGames { get; set; }
}