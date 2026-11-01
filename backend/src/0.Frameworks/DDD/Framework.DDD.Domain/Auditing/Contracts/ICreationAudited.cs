namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Shortcut for <see cref="ICreationAudited{TUserKey}"/> with <see cref="int"/> user key.
/// </summary>
public interface ICreationAudited : ICreationAudited<int>
{
}

/// <summary>
/// Entity stores who created it and when.
/// </summary>
public interface ICreationAudited<TUserKey> : IHasCreatedOnUtc
    where TUserKey : struct
{
    TUserKey CreatorUserId { get; set; }
}
