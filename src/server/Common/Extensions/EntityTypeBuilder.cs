using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Common.Extensions;

public static class EntityTypeBuilderExt
{
    public static ReferenceNavigationBuilder<TEntity, TRelatedEntity> HasOnly<TEntity, TRelatedEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TRelatedEntity?>>? navigationExpression = null)
        where TEntity : class
        where TRelatedEntity : class
        => builder.HasOne(navigationExpression);
}