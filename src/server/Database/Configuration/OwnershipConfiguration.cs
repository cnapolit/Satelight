using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class OwnershipConfiguration : IEntityTypeConfiguration<Ownership>
{
    public void Configure(EntityTypeBuilder<Ownership> builder)
        => builder.HasMany(o => o.References).WithOnly(g => g.Ownership).HasForeignKey(g => g.OwnershipId);
}
