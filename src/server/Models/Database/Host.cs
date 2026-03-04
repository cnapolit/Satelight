using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Database;

public class Host : DatabaseObject
{
    public required short    Port  { get; set; }
    // public ushort SunshinePort { get; set; } = 47990;
    public          DateTime Added { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(250, MinimumLength = 1)]
    public required string DnsName { get; set; }

    [StringLength(15, MinimumLength = 7)]
    public required string Ip { get; set; }

    [StringLength(12, MinimumLength = 12)]
    public required string MacAddress { get; set; }

    public string FormattedMacAddress
    {
        get
        {
            if (MacAddress.Length < 12) return string.Empty;
            var upperMac = MacAddress.ToUpper();
            return string.Join(":", Enumerable.Range(0, 6).Select(i => upperMac.Substring(i * 2, 2)));
        }
    }

    [StringLength(500)]
    public string OperatingSystem { get; set; } = string.Empty;

    [NotMapped]
    public string HostName => string.IsNullOrWhiteSpace(DisplayName) ? DnsName : DisplayName;

    public ICollection<Library>   Libraries  { get; set; } = [];
    public ICollection<HostGame>  Games      { get; set; } = [];
    public ICollection<Operation> Operations { get; set; } = [];
}