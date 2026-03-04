namespace Server.Models.Database;

public class CollectionLabel<T> : NamedDatabaseObject
{
    public ICollection<T> References { get; set; } = [];
}