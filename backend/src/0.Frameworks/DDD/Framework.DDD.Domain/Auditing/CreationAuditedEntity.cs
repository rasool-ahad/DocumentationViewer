using Framework.DDD.Domain.Auditing.Contracts;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Auditing;

/// <summary>
/// Shortcut: entity with <see cref="int"/> primary key and <see cref="int"/> user key.
/// </summary>
[Serializable]
public abstract class CreationAuditedEntity : CreationAuditedEntity<int, int>
{
}

/// <summary>
/// Entity that tracks who created it and when.
/// </summary>
[Serializable]
public abstract class CreationAuditedEntity<TPrimaryKey, TUserKey>
    : Entity<TPrimaryKey>, ICreationAudited<TUserKey>
    where TPrimaryKey : struct
    where TUserKey : struct
{
    public virtual DateTimeOffset CreatedOnUtc { get; set; }
    public virtual TUserKey CreatorUserId { get; set; }
}
