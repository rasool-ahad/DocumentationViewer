namespace Framework.Guard;

/// <summary>
/// A guard that throws a strongly-typed exception whenever a guard clause fails.
/// </summary>
/// <typeparam name="TException">The exception type thrown by this guard.</typeparam>
public class Guard<TException>
    where TException : Exception
{
    private readonly Func<string, TException> _exceptionFactory;

    public Guard(Func<string, TException> exceptionFactory)
    {
        _exceptionFactory = exceptionFactory;
    }

    internal void Throw(string message)
    {
        throw _exceptionFactory(message);
    }
}
