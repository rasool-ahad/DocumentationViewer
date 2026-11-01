namespace Framework.DDD.Domain.Exceptions;

public abstract class DomainStateException : Exception
{
    /// <summary>Optional parameter values substituted into the message template.</summary>
    public string[] Parameters { get; set; }

    protected DomainStateException(string message, params string[] parameters) : base(message)
    {
        Parameters = parameters;
    }

    public override string ToString()
    {
        if (Parameters is null || Parameters.Length == 0)
            return Message;

        string result = Message;
        for (int i = 0; i < Parameters.Length; i++)
            result = result.Replace($"{{{i}}}", Parameters[i]);
        return result;
    }
}
