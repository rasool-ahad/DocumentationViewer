namespace Framework.DDD.Application.Commands;

/// <summary>
/// Defines the command bus for dispatching commands.
/// </summary>
public interface ICommandBus
{
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class;
}

internal class CommandBus(IMediator mediator) : ICommandBus
{
    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
        => await mediator.Send(command, cancellationToken);

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class
        => await mediator.Send(command, cancellationToken);
}
