namespace Framework.DDD.Application.Queries;

/// <summary>
/// Marker interface for a query that returns a response.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : class
{
}
