namespace Framework.DDD.Application.Queries;

/// <summary>
/// Defines the query bus for dispatching queries.
/// </summary>
public interface IQueryBus
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class;
}

internal class QueryBus(IMediator mediator) : IQueryBus
{
    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class
        => await mediator.Send(query, cancellationToken);
}
