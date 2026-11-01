using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base role entity with int key (most common shortcut).
/// </summary>
public class BaseRole : BaseRole<int>
{
}

/// <summary>
/// Base role entity extending IdentityRole.
/// </summary>
/// <typeparam name="TKey">The primary key type.</typeparam>
public class BaseRole<TKey> : IdentityRole<TKey>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public bool IsTransient() => EqualityComparer<TKey>.Default.Equals(Id, default);
}
