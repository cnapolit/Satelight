using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.UserGameInfo).WithOnly(g => g.User).HasForeignKey(g => g.UserId);
        builder.HasMany(u => u.Accounts)    .WithOnly(a => a.User).HasForeignKey(a => a.UserId);
    }
}