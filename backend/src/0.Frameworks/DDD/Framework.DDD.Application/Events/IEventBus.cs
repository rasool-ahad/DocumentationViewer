using Framework.DDD.Domain.Events;

namespace Framework.DDD.Application.Events;

/// <summary>
/// Defines the event bus for publishing domain events.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;

    Task PublishAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

internal class EventBus(IPublisher publisher) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
        => await publisher.Publish(@event, cancellationToken);

    public async Task PublishAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        foreach (var @event in events)
            await publisher.Publish(@event, cancellationToken);
    }
}
