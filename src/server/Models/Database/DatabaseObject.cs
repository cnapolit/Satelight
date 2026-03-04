using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

public class DatabaseObject
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
}