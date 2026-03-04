
namespace Server.Models.Database;

public abstract class Alias
{
    public Guid   Id       { get; set; }
    public Guid   TargetId { get; set; }
    public string Name   { get; set; }
}