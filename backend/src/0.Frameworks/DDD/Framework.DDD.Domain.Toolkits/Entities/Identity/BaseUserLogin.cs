using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base user login entity with int key.
/// </summary>
public class BaseUserLogin : BaseUserLogin<int>
{
}

/// <summary>
/// Base user login entity.
/// </summary>
public class BaseUserLogin<TKey> : IdentityUserLogin<TKey>, IEntity
    where TKey : struct, IEquatable<TKey>
{
}
