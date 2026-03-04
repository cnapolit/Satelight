namespace Server.Models.Database;

public class WishListCategory : CollectionLabel<WishListGame>
{
    public User User { get; set; }
}