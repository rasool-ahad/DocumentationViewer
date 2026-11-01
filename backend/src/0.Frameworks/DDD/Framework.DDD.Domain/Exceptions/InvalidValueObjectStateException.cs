namespace Framework.DDD.Domain.Exceptions;

/// <summary>
/// Thrown when a value object is placed into an invalid state.
/// </summary>
public class InvalidValueObjectStateException : DomainStateException
{
    public InvalidValueObjectStateException(string message, params string[] parameters)
        : base(message, parameters)
    {
    }
}
