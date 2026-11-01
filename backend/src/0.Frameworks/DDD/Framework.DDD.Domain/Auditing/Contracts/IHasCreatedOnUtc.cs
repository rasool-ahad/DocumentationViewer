namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Entity tracks its UTC creation timestamp.
/// </summary>
public interface IHasCreatedOnUtc
{
    DateTimeOffset CreatedOnUtc { get; set; }
}
