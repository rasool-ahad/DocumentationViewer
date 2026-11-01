namespace Framework.Guard.GuardClauses;

public static class GreaterThanGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not greater than <paramref name="threshold"/>.
    /// </summary>
    public static void GreaterThan<T, TException>(
        this Guard<TException> guard,
        T value,
        T threshold,
        string message)
        where T : IComparable<T>
        where TException : Exception
    {
        if (value.CompareTo(threshold) <= 0)
            guard.Throw(message);
    }
}
