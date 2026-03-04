using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Host = Server.Models.Database.Host;

namespace Server.Database.Configuration;

public class HostConfiguration : IEntityTypeConfiguration<Host>
{
    public void Configure(EntityTypeBuilder<Host> builder)
    {
        builder.HasMany(h => h.Games)    .WithOnly(h => h.Host).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(h => h.Libraries).WithMany(l => l.Hosts);
    }
}