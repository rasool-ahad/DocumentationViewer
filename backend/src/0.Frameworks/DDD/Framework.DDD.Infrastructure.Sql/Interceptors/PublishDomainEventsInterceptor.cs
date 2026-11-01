using Framework.DDD.Application.Events;
using Framework.DDD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Framework.DDD.Infrastructure.Sql.Interceptors;

/// <summary>
/// Publishes aggregate domain events after a successful SaveChanges.
/// </summary>
public sealed class PublishDomainEventsInterceptor(IEventBus eventBus) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        PublishAndClearDomainEventsAsync(eventData.Context, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await PublishAndClearDomainEventsAsync(eventData.Context, cancellationToken);
        return result;
    }

    private async Task PublishAndClearDomainEventsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
            return;

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count > 0)
            .Select(x => new
            {
                Aggregate = x,
                Events = x.DomainEvents.ToList()
            })
            .ToList();

        if (aggregatesWithEvents.Count == 0)
            return;

        foreach (var item in aggregatesWithEvents)
        {
            foreach (var domainEvent in item.Events)
            {
                await eventBus.PublishAsync(domainEvent, cancellationToken);
                item.Aggregate.RemoveDomainEvent(domainEvent);
            }
        }
    }
}