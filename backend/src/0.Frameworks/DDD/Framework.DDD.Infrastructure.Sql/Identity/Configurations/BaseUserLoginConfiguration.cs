using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.DDD.Infrastructure.Sql.Identity.Configurations;

public class BaseUserLoginConfiguration : BaseUserLoginConfiguration<BaseUserLogin, int>
{
}

public class BaseUserLoginConfiguration<TUserLogin, TKey> : IEntityTypeConfiguration<TUserLogin>
    where TUserLogin : BaseUserLogin<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TUserLogin> builder)
    {
        builder.ToTable("UserLogins");
    }
}
