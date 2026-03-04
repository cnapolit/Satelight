using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Models.Database;

namespace Server.Database.Configuration;

public class CompanyConfiguration
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasMany(f => f.PublishedGames).WithMany(g => g.Publishers);
        builder.HasMany(f => f.DevelopedGames).WithMany(g => g.Developers);
    }
}
