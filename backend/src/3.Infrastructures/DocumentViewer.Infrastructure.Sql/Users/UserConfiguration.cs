using DocumentViewer.Domain.Users;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Framework.DDD.Infrastructure.Sql.Identity.Configurations;

namespace DocumentViewer.Infrastructure.Sql.Users;

public sealed class UserConfiguration : BaseUserConfiguration<User, BaseUserRole<int>, int>
{
}

