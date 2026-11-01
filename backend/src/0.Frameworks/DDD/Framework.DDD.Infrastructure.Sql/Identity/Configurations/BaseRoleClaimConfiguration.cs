using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

public class BaseRoleClaimConfiguration : BaseRoleClaimConfiguration<BaseRoleClaim, int>
{
}

public class BaseRoleClaimConfiguration<TRoleClaim, TKey> : IEntityTypeConfiguration<TRoleClaim>
    where TRoleClaim : BaseRoleClaim<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.ToTable("RoleClaims");
    }
}
