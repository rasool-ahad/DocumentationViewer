using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base user token entity with int key.
/// </summary>
public class BaseUserToken : BaseUserToken<int>
{
}

/// <summary>
/// Base user token entity.
/// </summary>
public class BaseUserToken<TKey> : IdentityUserToken<TKey>, IEntity
    where TKey : struct, IEquatable<TKey>
{
}
