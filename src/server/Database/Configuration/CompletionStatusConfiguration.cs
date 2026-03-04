using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class CompletionStatusConfiguration : IEntityTypeConfiguration<CompletionStatus>
{
    public void Configure(EntityTypeBuilder<CompletionStatus> builder)
    {
        builder.HasMany(u => u.References).WithOnly(u => u.CompletionStatus).HasForeignKey(u => u.CompletionStatusId);
    }
}
