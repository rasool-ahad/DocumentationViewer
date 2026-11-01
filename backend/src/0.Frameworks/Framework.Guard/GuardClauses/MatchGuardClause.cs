using System.Text.RegularExpressions;

namespace Framework.Guard.GuardClauses;

public static class MatchGuardClause
{
    /// <summary>
    /// Throws when <paramref name="value"/> does not match <paramref name="regexPattern"/>.
    /// </summary>
    public static void Match<TException>(
        this Guard<TException> guard,
        string value,
        string regexPattern,
        string message)
        where TException : Exception
    {
        if (value is null || !Regex.IsMatch(value, regexPattern))
            guard.Throw(message);
    }
}
