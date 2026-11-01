using Framework.DDD.Domain.Events;

namespace Framework.DDD.Domain.Entities;

/// <summary>
/// Contract for aggregate roots that manage domain events.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>Read-only list of uncommitted domain events raised by this aggregate.</summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>Appends a new domain event to the aggregate's event list.</summary>
    void AddDomainEvent(IDomainEvent newEvent);

    /// <summary>Removes a specific domain event from the aggregate's event list.</summary>
    void RemoveDomainEvent(IDomainEvent eventItem);

    /// <summary>Clears all domain events — typically called after they have been dispatched.</summary>
    void ClearEvents();
}
