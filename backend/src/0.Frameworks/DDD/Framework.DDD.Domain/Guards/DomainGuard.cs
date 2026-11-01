using Framework.DDD.Domain.Exceptions;
using Framework.Guard;

namespace Framework.DDD.Domain.Guards;

public static class DomainGuard
{
    /// <summary>
    /// A ready-to-use guard that throws <see cref="DomainException"/> when a clause fails.
    /// The guard is stateless, so a single shared instance is safe.
    /// </summary>
    public static Guard<DomainException> ThrowIf { get; } =
        new(message => new DomainException(message));
}
