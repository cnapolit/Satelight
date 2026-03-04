namespace Server.Models.Database;

public class GameOwnership
{
    public Guid AccountId     { get; set; }
    public Guid LibraryGameId { get; set; }
    public Guid OwnershipId   { get; set; }

    public Account     Account     { get; set; }
    public LibraryGame LibraryGame { get; set; }
    public Ownership   Ownership   { get; set; }
}