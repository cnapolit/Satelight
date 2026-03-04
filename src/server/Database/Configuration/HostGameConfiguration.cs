using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class HostGameConfiguration : IEntityTypeConfiguration<HostGame>
{
    public void Configure(EntityTypeBuilder<HostGame> builder)
    {
        builder.HasOnly(h => h.Host)       .WithMany(h => h.Games)    .HasForeignKey(hg => hg.HostId);
        builder.HasOnly(h => h.LibraryGame).WithMany(l => l.HostGames).HasForeignKey(hg => hg.LibraryGameId);
    }
}