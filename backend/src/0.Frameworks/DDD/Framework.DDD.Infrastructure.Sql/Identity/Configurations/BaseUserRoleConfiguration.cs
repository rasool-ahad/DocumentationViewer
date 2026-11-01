using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

public class BaseUserRoleConfiguration : BaseUserRoleConfiguration<BaseUserRole, int>
{
}

public class BaseUserRoleConfiguration<TUserRole, TKey> : IEntityTypeConfiguration<TUserRole>
    where TUserRole : BaseUserRole<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.ToTable("UserRoles");
    }
}
