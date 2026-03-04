using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasMany(a => a.OwnedGames).WithOnly(g => g.Account);
        builder.HasOnly(a => a.User)      .WithMany(u => u.Accounts).OnDelete(DeleteBehavior.Cascade).HasForeignKey(a => a.UserId);
        builder.HasOnly(a => a.Library)   .WithMany(l => l.Accounts).OnDelete(DeleteBehavior.Cascade).HasForeignKey(a => a.LibraryId);
    }
}