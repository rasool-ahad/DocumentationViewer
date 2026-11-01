using Framework.DDD.Domain.Auditing.Contracts;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: entity with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class AuditedEntity : AuditedEntity<int, int>
{
}

/// <summary>
/// Entity that tracks creation and last modification auditing.
/// </summary>
[Serializable]
public abstract class AuditedEntity<TPrimaryKey, TUserKey>
    : CreationAuditedEntity<TPrimaryKey, TUserKey>, IAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual DateTimeOffset? UpdatedOnUtc { get; set; }
    public virtual TUserKey? LastModifierUserId { get; set; }
}
