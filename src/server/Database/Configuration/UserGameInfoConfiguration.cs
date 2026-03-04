using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class UserGameInfoConfiguration : IEntityTypeConfiguration<UserGameInfo>
{
    public void Configure(EntityTypeBuilder<UserGameInfo> builder)
    {
        builder.HasOnly(u => u.User)            .WithMany(u => u.UserGameInfo).OnDelete(DeleteBehavior.Cascade);
        builder.HasOnly(u => u.Game)            .WithMany(u => u.UserGameInfo).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.UserGameSessions).WithOnly(u => u.UserGameInfo);
    }
}