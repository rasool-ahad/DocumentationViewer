namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Shortcut for <see cref="IAudited{TUserKey}"/> with <see cref="int"/> user key.
/// </summary>
public interface IAudited : ICreationAudited, IModificationAudited
{
}

/// <summary>
/// Entity tracks both creation and last modification auditing.
/// </summary>
public interface IAudited<TUserKey> : ICreationAudited<TUserKey>, IModificationAudited<TUserKey>
    where TUserKey : struct
{
}
