using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Server.Models.Database;

[Index(nameof(LibraryId), nameof(UserName), IsUnique = true)]
public class Account : DatabaseObject
{
    public Guid UserId { get; set; }
    public Guid LibraryId { get; set; }

    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500)]
    public required string UserName { get; set; }

    public User                       User       { get; set; }
    public Library                    Library    { get; set; }
    public ICollection<GameOwnership> OwnedGames { get; set; } = [];
}