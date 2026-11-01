namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Shortcut for <see cref="IFullAudited{TUserKey}"/> with <see cref="int"/> user key.
/// </summary>
public interface IFullAudited : IFullAudited<int>
{
}

/// <summary>
/// Entity tracks creation, modification, and soft-deletion auditing.
/// </summary>
public interface IFullAudited<TUserKey> : IAudited<TUserKey>, IDeletionAudited<TUserKey>
    where TUserKey : struct
{
}
