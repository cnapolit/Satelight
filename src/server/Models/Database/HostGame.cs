using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

[Index(nameof(HostId), nameof(LibraryGameId), IsUnique = true)]
public class HostGame : DatabaseObject
{
    public Guid  HostId        { get; set; }
    public Guid  LibraryGameId { get; set; }
    public bool  Installed     { get; set; }
    public bool  Playing       { get; set; }
    public long? Size          { get; set; }


    [StringLength(100, MinimumLength = 1)]
    public required string HostGameId { get; set; }

    [StringLength(1000)]
    public string InstallPath { get; set; } = string.Empty;


    [StringLength(100)]
    public string Version { get; set; } = string.Empty;

    public Host        Host        { get; set; }
    public LibraryGame LibraryGame { get; set; }
}