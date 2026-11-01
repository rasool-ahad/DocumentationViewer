namespace Framework.DDD.Presentation.Models;

/// <summary>
/// Wrapper for API requests that includes a client context alongside the data payload.
/// Used by the request middleware to extract context from non-GET bodies.
/// </summary>
public class ApiRequestWrapper<TRequest>
{
    public string? Context { get; set; }
    public TRequest? Data { get; set; }
}
