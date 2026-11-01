using MediatR;

namespace Framework.DDD.Domain.Events;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain and are
/// used for state-change propagation within and across aggregates.
/// </summary>
public interface IDomainEvent : INotification
{
}
