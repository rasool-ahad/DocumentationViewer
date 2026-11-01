using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Framework.DDD.Domain.Auditing.Contracts;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Events;

namespace Framework.DDD.Domain.Toolkits.Entities.Identity;

/// <summary>
/// Base user entity with int key (most common shortcut).
/// </summary>
public class BaseUser : BaseUser<int>
{
}

/// <summary>
/// Base user entity extending IdentityUser with full auditing and soft delete.
/// </summary>
/// <typeparam name="TKey">The primary key type.</typeparam>
public class BaseUser<TKey> : IdentityUser<TKey>,
    IHasCreatedOnUtc,
    IModificationAudited<TKey>,
    IDeletionAudited<TKey>,
    IAggregateRoot
    where TKey : struct, IEquatable<TKey>
{
    public string FirstName { get; protected set; } = string.Empty;

    public string LastName { get; protected set; } = string.Empty;

    public bool IsActive { get; protected set; }

    public bool Blocked { get; protected set; }

    public DateTimeOffset? LastLoggedOnUtc { get; protected set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // IHasCreatedOnUtc
    public DateTimeOffset CreatedOnUtc { get; set; }

    // IModificationAudited
    public DateTimeOffset? UpdatedOnUtc { get; set; }
    public TKey? LastModifierUserId { get; set; }

    // IDeletionAudited (includes ISoftDelete + IHasDeletedOnUtc)
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOnUtc { get; set; }
    public TKey? DeleterUserId { get; set; }

    #region AggregateRoot
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    private readonly List<IDomainEvent> _domainEvents = [];

    public void AddDomainEvent(IDomainEvent newEvent)
    {
        if (_domainEvents == null)
        {
            throw new ArgumentNullException(nameof(newEvent));
        }

        _domainEvents.Add(newEvent);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        if (_domainEvents == null)
        {
            throw new ArgumentNullException(nameof(eventItem));
        }

        _domainEvents.Remove(eventItem);
    }
    #endregion
}
