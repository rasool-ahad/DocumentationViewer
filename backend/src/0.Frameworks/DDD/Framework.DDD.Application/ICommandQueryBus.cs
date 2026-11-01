using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Queries;

namespace Framework.DDD.Application;

/// <summary>
/// Unified bus for dispatching both commands and queries.
/// </summary>
public interface ICommandQueryBus
{
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class;

    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class;
}

internal class CommandQueryBus(ICommandBus commandBus, IQueryBus queryBus) : ICommandQueryBus
{
    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
        => await commandBus.SendAsync(command, cancellationToken);

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class
        => await commandBus.SendAsync(command, cancellationToken);

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class
        => await queryBus.SendAsync(query, cancellationToken);
}
