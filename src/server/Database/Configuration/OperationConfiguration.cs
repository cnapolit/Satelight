using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {}//=> builder.HasOnly(o => o.Host).WithMany(h => h.Operations).HasForeignKey(o => o.Host).OnDelete(DeleteBehavior.Cascade);
}
