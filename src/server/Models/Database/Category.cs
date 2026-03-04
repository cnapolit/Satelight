namespace Server.Models.Database;

public class Category : CollectionLabel<UserGameInfo>
{
    public User User { get; set; }
}