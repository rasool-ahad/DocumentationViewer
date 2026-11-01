namespace Framework.DDD.Domain.Exceptions;

/// <summary>
/// Thrown when an entity is placed into an invalid state.
/// </summary>
public class InvalidEntityStateException : DomainStateException
{
    public InvalidEntityStateException(string message, params string[] parameters)
        : base(message, parameters)
    {
    }
}
