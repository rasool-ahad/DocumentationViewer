using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base user role join entity with int key.
/// </summary>
public class BaseUserRole : BaseUserRole<int>
{
}

/// <summary>
/// Base user role join entity.
/// </summary>
public class BaseUserRole<TKey> : IdentityUserRole<TKey>, IEntity
    where TKey : struct, IEquatable<TKey>
{
}
