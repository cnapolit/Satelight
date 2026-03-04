using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Common.Extensions;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class GameOwnershipConfiguration : IEntityTypeConfiguration<GameOwnership>
{
    public void Configure(EntityTypeBuilder<GameOwnership> builder)
    {
        builder.HasKey(g => new { g.AccountId, g.LibraryGameId });
        builder.HasOnly(g => g.Account)    .WithMany(a => a.OwnedGames)   .HasForeignKey(g => g.AccountId)    .OnDelete(DeleteBehavior.Cascade);
        builder.HasOnly(g => g.LibraryGame).WithMany(l => l.AccountOwners).HasForeignKey(g => g.LibraryGameId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOnly(g => g.Ownership)  .WithMany(g => g.References)   .HasForeignKey(g => g.OwnershipId);
    }
}
