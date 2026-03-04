using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class LibraryGameConfiguration : IEntityTypeConfiguration<LibraryGame>
{
    public void Configure(EntityTypeBuilder<LibraryGame> builder)
    {
        builder.HasOnly(l => l.Library)         .WithMany(l => l.LibraryGames);
        builder.HasOnly(l => l.GameVariant)     .WithMany(g => g.LibraryGames);
        builder.HasMany(l => l.GameSessions).WithOnly(u => u.LibraryGame).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(l => l.HostGames)       .WithOnly(h => h.LibraryGame).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(l => l.AccountOwners)   .WithOnly(g => g.LibraryGame).OnDelete(DeleteBehavior.Cascade);
    }
}