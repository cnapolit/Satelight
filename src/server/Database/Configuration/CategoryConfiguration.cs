using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public virtual void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasMany(f => f.References).WithMany(g => g.Categories);
        builder.HasOnly(c => c.User)      .WithMany(u => u.Categories).OnDelete(DeleteBehavior.Cascade);
    }
}