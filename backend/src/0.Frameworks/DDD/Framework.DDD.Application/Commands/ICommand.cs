namespace Framework.DDD.Application.Commands;

/// <summary>
/// Marker interface for a command without a response.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for a command that returns a response.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse> where TResponse : class
{
}
