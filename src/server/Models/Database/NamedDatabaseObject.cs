using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.Database;

[Index(nameof(Name), IsUnique = true)]
public class NamedDatabaseObject : DatabaseObject
{
    [StringLength(500, MinimumLength = 1)]
    public string Name { get; set; }
}