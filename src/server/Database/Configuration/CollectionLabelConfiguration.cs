using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Models.Database;

namespace Server.Database.Configuration;

public abstract class CollectionLabelConfiguration<TEntity, TRelatedEntity>(
    Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>> navigationExpression) : IEntityTypeConfiguration<TEntity>
    where TEntity        : CollectionLabel<TRelatedEntity>
    where TRelatedEntity : class
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        => builder.HasMany(f => f.References).WithMany(navigationExpression);
}