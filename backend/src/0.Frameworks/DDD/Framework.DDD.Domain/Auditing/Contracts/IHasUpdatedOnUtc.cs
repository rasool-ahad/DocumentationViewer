namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Entity tracks its last UTC modification timestamp.
/// </summary>
public interface IHasUpdatedOnUtc
{
    DateTimeOffset? UpdatedOnUtc { get; set; }
}
