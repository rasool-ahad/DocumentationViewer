namespace Framework.Guard.GuardClauses;

public static class NullGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not null.
    /// </summary>
    public static void Null<T, TException>(
        this Guard<TException> guard,
        T value,
        string message)
        where TException : Exception
    {
        if (value is not null)
            guard.Throw(message);
    }
}
