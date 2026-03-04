namespace Server.Models.Database;

public class Library : NamedDatabaseObject
{
    public ICollection<Host>        Hosts        { get; set; } = [];
    public ICollection<LibraryGame> LibraryGames { get; set; } = [];
    public ICollection<Account>     Accounts     { get; set; } = [];
}