using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Common.Extensions;

public static class CollectionNavigationBuilderExt
{
    public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> WithOnly<TEntity, TRelatedEntity>(
        this CollectionNavigationBuilder<TEntity, TRelatedEntity> builder,
        Expression<Func<TRelatedEntity, TEntity?>>? navigationExpression)
        where TEntity : class
        where TRelatedEntity : class
        => builder.WithOne(navigationExpression);

    public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> WithOnly<TEntity, TRelatedEntity>(
        this CollectionNavigationBuilder<TEntity, TRelatedEntity> builder)
        where TEntity : class
        where TRelatedEntity : class
        => builder.WithOne();
}