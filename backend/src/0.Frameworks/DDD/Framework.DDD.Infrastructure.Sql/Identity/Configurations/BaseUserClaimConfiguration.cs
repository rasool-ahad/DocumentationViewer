using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

public class BaseUserClaimConfiguration : BaseUserClaimConfiguration<BaseUserClaim, int>
{
}

public class BaseUserClaimConfiguration<TUserClaim, TKey> : IEntityTypeConfiguration<TUserClaim>
    where TUserClaim : BaseUserClaim<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable("UserClaims");
    }
}
