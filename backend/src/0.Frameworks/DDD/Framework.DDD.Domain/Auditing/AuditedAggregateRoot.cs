using Framework.DDD.Domain.Auditing.Contracts;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: aggregate root with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class AuditedAggregateRoot : AuditedAggregateRoot<int, int>
{
}

/// <summary>
/// Aggregate root that tracks creation and last modification auditing.
/// </summary>
[Serializable]
public abstract class AuditedAggregateRoot<TPrimaryKey, TUserKey>
    : CreationAuditedAggregateRoot<TPrimaryKey, TUserKey>, IAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual DateTimeOffset? UpdatedOnUtc { get; set; }
    public virtual TUserKey? LastModifierUserId { get; set; }
}
