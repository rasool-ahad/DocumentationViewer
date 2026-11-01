using System.Collections;

namespace Framework.Guard.GuardClauses;

public static class EmptyGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not empty.
    /// </summary>
    public static void Empty<T, TException>(
        this Guard<TException> guard,
        T value,
        string message)
        where TException : Exception
    {
        if (value is ICollection { Count: > 0 })
            guard.Throw(message);

        if (value is string str && !string.IsNullOrWhiteSpace(str))
            guard.Throw(message);

        if (!EqualityComparer<T>.Default.Equals(value, default))
            guard.Throw(message);
    }

    /// <summary>
    /// Throws when <paramref name="value"/> is empty.
    /// </summary>
    public static void NotEmpty<T, TException>(
        this Guard<TException> guard,
        T value,
        string message)
        where TException : Exception
    {
        if (value == null)
            guard.Throw(message);

        if (value is ICollection collection && collection.Count == 0)
            guard.Throw(message);

        if (value is string str && string.IsNullOrWhiteSpace(str))
            guard.Throw(message);

        if (EqualityComparer<T>.Default.Equals(value, default))
            guard.Throw(message);
    }
}
