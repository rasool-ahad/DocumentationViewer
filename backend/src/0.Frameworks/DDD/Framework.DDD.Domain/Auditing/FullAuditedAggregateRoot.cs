using Framework.DDD.Domain.Auditing.Contracts;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: aggregate root with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class FullAuditedAggregateRoot : FullAuditedAggregateRoot<int, int>
{
}

/// <summary>
/// Aggregate root with full auditing: creation, modification, and soft-deletion.
/// </summary>
[Serializable]
public abstract class FullAuditedAggregateRoot<TPrimaryKey, TUserKey>
    : AuditedAggregateRoot<TPrimaryKey, TUserKey>, IFullAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual bool IsDeleted { get; set; }
    public virtual TUserKey? DeleterUserId { get; set; }
    public virtual DateTimeOffset? DeletedOnUtc { get; set; }
}
