using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class UserGameSessionConfiguration : IEntityTypeConfiguration<UserGameSession>
{
    public void Configure(EntityTypeBuilder<UserGameSession> builder)
    {
        builder.HasOnly(u => u.UserGameInfo).WithMany(u => u.UserGameSessions).OnDelete(DeleteBehavior.Cascade);
        builder.HasOnly(u => u.LibraryGame) .WithMany(u => u.GameSessions)    .OnDelete(DeleteBehavior.Cascade);
    }
}