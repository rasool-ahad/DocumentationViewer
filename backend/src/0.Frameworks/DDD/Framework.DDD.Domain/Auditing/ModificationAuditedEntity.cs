using Framework.DDD.Domain.Auditing.Contracts;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: entity with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class ModificationAuditedEntity : ModificationAuditedEntity<int, int>
{
}

/// <summary>
/// Entity that tracks who last modified it and when.
/// </summary>
[Serializable]
public abstract class ModificationAuditedEntity<TPrimaryKey, TUserKey>
    : Entity<TPrimaryKey>, IModificationAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual DateTimeOffset? UpdatedOnUtc { get; set; }
    public virtual TUserKey? LastModifierUserId { get; set; }
}
