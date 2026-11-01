namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Entity tracks its UTC soft-deletion timestamp. Extends <see cref="ISoftDelete"/>.
/// </summary>
public interface IHasDeletedOnUtc : ISoftDelete
{
    DateTimeOffset? DeletedOnUtc { get; set; }
}
