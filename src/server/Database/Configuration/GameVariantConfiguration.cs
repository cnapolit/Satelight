using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class GameVariantConfiguration : IEntityTypeConfiguration<GameVariant>
{
    public void Configure(EntityTypeBuilder<GameVariant> builder)
    {
        builder.HasMany(g => g.LibraryGames)   .WithOnly(l => l.GameVariant);
        builder.HasMany(g => g.Games)          .WithMany(g => g.GameVariants);
        builder.HasMany(g => g.Features)       .WithMany(f => f.References);
        builder.HasOnly(g => g.GameVariantType).WithMany(g => g.References);
        builder.HasMany(g => g.Developers)     .WithMany(c => c.DevelopedGames);
        builder.HasMany(g => g.Publishers)     .WithMany(c => c.PublishedGames);
        builder.HasMany(g => g.Platforms)      .WithMany(g => g.References);
    }
}