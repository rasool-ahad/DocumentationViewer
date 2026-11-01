using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base user claim entity with int key.
/// </summary>
public class BaseUserClaim : BaseUserClaim<int>
{
}

/// <summary>
/// Base user claim entity.
/// </summary>
public class BaseUserClaim<TKey> : IdentityUserClaim<TKey>, IEntity
    where TKey : struct, IEquatable<TKey>
{
}
