namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Shortcut for <see cref="IModificationAudited{TUserKey}"/> with <see cref="int"/> user key.
/// </summary>
public interface IModificationAudited : IModificationAudited<int>
{
}

/// <summary>
/// Entity stores who last modified it and when.
/// </summary>
public interface IModificationAudited<TUserKey> : IHasUpdatedOnUtc
    where TUserKey : struct
{
    TUserKey? LastModifierUserId { get; set; }
}
