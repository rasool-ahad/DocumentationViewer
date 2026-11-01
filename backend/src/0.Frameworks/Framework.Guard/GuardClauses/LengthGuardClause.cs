namespace Framework.Guard.GuardClauses;

public static class LengthGuardClause
{
    /// <summary>
    /// Throws when the length of <paramref name="value"/> does not equal <paramref name="length"/>.
    /// </summary>
    public static void Length<TException>(
        this Guard<TException> guard,
        string value,
        int length,
        string message)
        where TException : Exception
    {
        if (value?.Length != length)
            guard.Throw(message);
    }

    /// <summary>
    /// Throws when the length of <paramref name="value"/> is less than <paramref name="minimumLength"/>.
    /// </summary>
    public static void MinimumLength<TException>(
        this Guard<TException> guard,
        string value,
        int minimumLength,
        string message)
        where TException : Exception
    {
        if (value is null || value.Length < minimumLength)
            guard.Throw(message);
    }
}
