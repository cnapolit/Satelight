using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

[Index(nameof(LibraryId), nameof(LibraryGameId), IsUnique = true)]
public class LibraryGame : DatabaseObject
{
    public Guid LibraryId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public required string LibraryGameId { get; set; }

    public Library                      Library       { get; set; }
    public GameVariant                  GameVariant   { get; set; }
    public ICollection<UserGameSession> GameSessions  { get; set; }
    public ICollection<HostGame>        HostGames     { get; set; }
    public ICollection<GameOwnership>   AccountOwners { get; set; }
}