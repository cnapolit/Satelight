namespace Server.Models.Database;

public class GameVariantType : CollectionLabel<GameVariant>
{
    public GameVariantFlags GameVariantFlags { get; set; }
}