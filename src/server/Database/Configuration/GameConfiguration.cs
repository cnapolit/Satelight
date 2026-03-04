using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasMany(g => g.GameVariants).WithMany(g => g.Games);
        builder.HasMany(g => g.UserGameInfo).WithOnly(u => u.Game);
        builder.HasMany(g => g.Genres)      .WithMany(g => g.References);
        builder.HasMany(g => g.Tags)        .WithMany(t => t.References);
        builder.HasMany(g => g.Series)      .WithMany(s => s.References);
    }
}