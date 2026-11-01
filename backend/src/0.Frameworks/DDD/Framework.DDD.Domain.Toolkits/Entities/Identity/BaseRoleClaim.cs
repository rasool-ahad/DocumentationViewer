using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base role claim entity with int key.
/// </summary>
public class BaseRoleClaim : BaseRoleClaim<int>
{
}

/// <summary>
/// Base role claim entity.
/// </summary>
public class BaseRoleClaim<TKey> : IdentityRoleClaim<TKey>, IEntity
    where TKey : struct, IEquatable<TKey>
{
}
