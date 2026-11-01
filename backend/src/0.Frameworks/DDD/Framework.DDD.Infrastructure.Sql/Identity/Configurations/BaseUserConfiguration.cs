using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

/// <summary>
/// Configuration for BaseUser entity with int key (shortcut).
/// </summary>
public class BaseUserConfiguration : BaseUserConfiguration<BaseUser, BaseUserRole, int>
{
}

/// <summary>
/// Configuration for BaseUser entity.
/// </summary>
public class BaseUserConfiguration<TUser, TUserRole, TKey> : IEntityTypeConfiguration<TUser>
    where TUser : BaseUser<TKey>
    where TUserRole : BaseUserRole<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder.ToTable("Users");
        builder.Property(p => p.FirstName).HasMaxLength(128);
        builder.Property(p => p.LastName).HasMaxLength(128);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.CreatedOnUtc).HasDefaultValueSql("GETUTCDATE()");
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
