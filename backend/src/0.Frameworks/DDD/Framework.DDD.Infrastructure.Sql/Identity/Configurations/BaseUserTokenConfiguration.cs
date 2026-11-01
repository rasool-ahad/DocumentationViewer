using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

public class BaseUserTokenConfiguration : BaseUserTokenConfiguration<BaseUserToken, int>
{
}

public class BaseUserTokenConfiguration<TUserToken, TKey> : IEntityTypeConfiguration<TUserToken>
    where TUserToken : BaseUserToken<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TUserToken> builder)
    {
        builder.ToTable("UserTokens");
    }
}
