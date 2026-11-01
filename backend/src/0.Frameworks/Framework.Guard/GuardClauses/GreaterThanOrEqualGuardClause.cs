namespace Framework.Guard.GuardClauses;

public static class GreaterThanOrEqualGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not greater than or equal to <paramref name="threshold"/>.
    /// </summary>
    public static void GreaterThanOrEqual<T, TException>(
        this Guard<TException> guard,
        T value,
        T threshold,
        string message)
        where T : IComparable<T>
        where TException : Exception
    {
        if (value.CompareTo(threshold) < 0)
            guard.Throw(message);
    }
}
