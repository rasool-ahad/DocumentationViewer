namespace Framework.DDD.Domain.Exceptions;

/// <summary>
/// Represents a generic domain-level error.
/// </summary>
public class DomainException : DomainStateException
{
    public DomainException(string message, params string[] parameters)
        : base(message, parameters)
    {
    }
}
