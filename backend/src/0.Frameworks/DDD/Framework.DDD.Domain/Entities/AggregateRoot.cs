using Framework.DDD.Domain.Events;

namespace Framework.DDD.Domain.Entities;

/// <summary>
/// Aggregate root with an <see cref="int"/> primary key.
/// Inherit this for the most common case.
/// </summary>
public abstract class AggregateRoot : AggregateRoot<int>
{
}

/// <summary>
/// Aggregate root with a generic single-value primary key.
/// Extends <see cref="Entity{TPrimaryKey}"/> and implements <see cref="IAggregateRoot"/>.
/// </summary>
/// <typeparam name="TPrimaryKey">Type of the primary key (e.g. int, Guid, long).</typeparam>
public abstract class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot
    where TPrimaryKey : struct
{
    private readonly List<IDomainEvent> _domainEvents;

    public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected AggregateRoot()
    {
        _domainEvents = new List<IDomainEvent>();
    }

    public virtual void AddDomainEvent(IDomainEvent newEvent)
    {
        ArgumentNullException.ThrowIfNull(newEvent);
        _domainEvents.Add(newEvent);
    }

    public virtual void RemoveDomainEvent(IDomainEvent eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);
        _domainEvents.Remove(eventItem);
    }

    public virtual void ClearEvents() => _domainEvents.Clear();
}
