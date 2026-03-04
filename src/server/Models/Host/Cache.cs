using Satelight.Protos.Core;

namespace Server.Models.Host;

public class Cache
{
    public ICollection<Label> Genres    { get; set; } = [];
    public ICollection<Label> Tags      { get; set; } = [];
    public ICollection<Label> Platforms { get; set; } = [];
    public ICollection<Label> Features  { get; set; } = [];
    public ICollection<Label> Companies { get; set; } = [];
    public ICollection<Label> Series    { get; set; } = [];
    public ICollection<Label> Libraries { get; set; } = [];
}