using Framework.DDD.Domain.Events;

namespace Framework.DDD.Application.Events;

/// <summary>
/// Defines a handler for a domain event.
/// </summary>
/// <typeparam name="TEvent">The domain event type.</typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
