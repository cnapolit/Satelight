using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class GameVariantTypeConfiguration : IEntityTypeConfiguration<GameVariantType>
{
    public void Configure(EntityTypeBuilder<GameVariantType> builder)
    {
        builder.HasMany(g => g.References).WithOnly(g => g.GameVariantType);
    }
}