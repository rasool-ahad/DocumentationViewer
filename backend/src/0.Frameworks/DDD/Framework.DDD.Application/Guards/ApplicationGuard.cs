using Framework.DDD.Application.Exceptions;
using Framework.Guard;

namespace Framework.DDD.Application.Guards;

public static class ApplicationGuard
{
    /// <summary>
    /// A ready-to-use guard that throws <see cref="ApplicationBadRequestException"/> when a clause fails.
    /// The guard is stateless, so a single shared instance is safe.
    /// </summary>
    public static Guard<ApplicationBadRequestException> ThrowIf { get; } =
        new(message => new ApplicationBadRequestException(message));
}
