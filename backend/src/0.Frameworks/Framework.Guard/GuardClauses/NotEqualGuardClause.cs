namespace Framework.Guard.GuardClauses;

public static class NotEqualGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> equals <paramref name="expected"/>.
    /// </summary>
    public static void NotEqual<T, TException>(
        this Guard<TException> guard,
        T value,
        T expected,
        string message)
        where TException : Exception
    {
        if (EqualityComparer<T>.Default.Equals(value, expected))
            guard.Throw(message);
    }
}
