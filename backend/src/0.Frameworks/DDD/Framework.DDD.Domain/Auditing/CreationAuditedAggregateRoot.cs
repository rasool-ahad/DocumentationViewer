using Framework.DDD.Domain.Auditing.Contracts;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: aggregate root with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class CreationAuditedAggregateRoot : CreationAuditedAggregateRoot<int, int>
{
}

/// <summary>
/// Aggregate root that tracks creation auditing and manages domain events.
/// </summary>
[Serializable]
public abstract class CreationAuditedAggregateRoot<TPrimaryKey, TUserKey>
    : AggregateRoot<TPrimaryKey>, ICreationAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual DateTimeOffset CreatedOnUtc { get; set; }
    public virtual TUserKey CreatorUserId { get; set; }
}
