using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

/// <summary>
/// Configuration for BaseRole entity with int key (shortcut).
/// </summary>
public class BaseRoleConfiguration : BaseRoleConfiguration<BaseRole, int>
{
}

/// <summary>
/// Configuration for BaseRole entity.
/// </summary>
public class BaseRoleConfiguration<TRole, TKey> : IEntityTypeConfiguration<TRole>
    where TRole : BaseRole<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TRole> builder)
    {
        builder.ToTable("Roles");
    }
}
