namespace Framework.Guard.GuardClauses;

public static class LessThanOrEqualGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not less than or equal to <paramref name="threshold"/>.
    /// </summary>
    public static void LessThanOrEqual<T, TException>(
        this Guard<TException> guard,
        T value,
        T threshold,
        string message)
        where T : IComparable<T>
        where TException : Exception
    {
        if (value.CompareTo(threshold) > 0)
            guard.Throw(message);
    }
}
