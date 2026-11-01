namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Shortcut for <see cref="IDeletionAudited{TUserKey}"/> with <see cref="int"/> user key.
/// </summary>
public interface IDeletionAudited : IDeletionAudited<int>
{
}

/// <summary>
/// Entity stores who soft-deleted it and when.
/// </summary>
public interface IDeletionAudited<TUserKey> : IHasDeletedOnUtc
    where TUserKey : struct
{
    TUserKey? DeleterUserId { get; set; }
}
