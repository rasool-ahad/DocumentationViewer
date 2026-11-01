namespace Framework.Guard.GuardClauses;

public static class LessThanGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> is not less than <paramref name="threshold"/>.
    /// </summary>
    public static void LessThan<T, TException>(
        this Guard<TException> guard,
        T value,
        T threshold,
        string message)
        where T : IComparable<T>
        where TException : Exception
    {
        if (value.CompareTo(threshold) >= 0)
            guard.Throw(message);
    }
}
