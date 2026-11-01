using Framework.DDD.Domain.Auditing.Contracts;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: entity with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class FullAuditedEntity : FullAuditedEntity<int, int>
{
}

/// <summary>
/// Entity with full auditing: creation, modification, and soft-deletion.
/// </summary>
[Serializable]
public abstract class FullAuditedEntity<TPrimaryKey, TUserKey>
    : AuditedEntity<TPrimaryKey, TUserKey>, IFullAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual bool IsDeleted { get; set; }
    public virtual TUserKey? DeleterUserId { get; set; }
    public virtual DateTimeOffset? DeletedOnUtc { get; set; }
}
