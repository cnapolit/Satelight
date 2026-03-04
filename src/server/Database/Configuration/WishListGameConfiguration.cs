using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class WishListGameConfiguration : IEntityTypeConfiguration<WishListGame>
{
    public void Configure(EntityTypeBuilder<WishListGame> builder)
    {
        builder.HasOnly(w => w.Game)      .WithMany(g => g.WishListEntries).OnDelete(DeleteBehavior.Cascade);
        builder.HasOnly(w => w.User)      .WithMany(u => u.WishList)       .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(w => w.Categories).WithMany(c => c.References);
    }
}
