namespace Server.Models.Database;

public class WishListGame : DatabaseObject
{
    public int? Rank { get; set; }

    public User User { get; set; }
    public Game Game { get; set; }
    public ICollection<WishListCategory> Categories { get; set; } = [];
}