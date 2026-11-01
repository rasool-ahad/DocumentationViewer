using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Infrastructure.Sql.Configurations;

/// <summary>
/// Base entity type configuration for entities with int primary key (most common).
/// </summary>
public abstract class EntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity, int>
    where TEntity : class, IEntity<int>
{
}

/// <summary>
/// Base entity type configuration for entities with a specified primary key type.
/// </summary>
public abstract class EntityTypeConfiguration<TEntity, TPrimaryKey> : IEntityTypeConfiguration<TEntity>
    where TPrimaryKey : struct
    where TEntity : class, IEntity<TPrimaryKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name, "dbo");
    }
}
