using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Auditing.Contracts;

namespace Framework.DDD.Infrastructure.Sql.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Registers all entity types implementing IEntity from the specified assembly.
    /// </summary>
    public static void RegisterAllEntities(this ModelBuilder modelBuilder, Assembly assembly)
    {
        assembly.GetExportedTypes()
            .Where(p =>
                p is { IsPublic: true, IsClass: true, IsAbstract: false } &&
                typeof(IEntity).IsAssignableFrom(p))
            .ToList()
            .ForEach(p => modelBuilder.Entity(p));
    }

    /// <summary>
    /// Applies auditing concepts (creation, modification, soft-delete) to tracked entities.
    /// </summary>
    public static void ApplyConcepts<TUserKey>(this ChangeTracker changeTracker, TUserKey? auditUserId)
        where TUserKey : struct
    {
        var entries = changeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreationAudit(entry, auditUserId);
                    break;

                case EntityState.Modified:
                    SetModificationAudit(entry, auditUserId);
                    if (entry.Entity is ISoftDelete softDelete && softDelete.IsDeleted)
                        SetDeletionAudit(entry, auditUserId);
                    break;

                case EntityState.Deleted:
                    HandleSoftDelete(entry, auditUserId);
                    break;
            }
        }
    }

    private static void SetCreationAudit<TUserKey>(EntityEntry entry, TUserKey? auditUserId)
        where TUserKey : struct
    {
        if (entry.Entity is IHasCreatedOnUtc hasCreated)
            hasCreated.CreatedOnUtc = DateTimeOffset.UtcNow;

        if (auditUserId.HasValue && entry.Entity is ICreationAudited<TUserKey> creationAudited)
            creationAudited.CreatorUserId = auditUserId.Value;
    }

    private static void SetModificationAudit<TUserKey>(EntityEntry entry, TUserKey? auditUserId)
        where TUserKey : struct
    {
        if (entry.Entity is IHasUpdatedOnUtc hasUpdated)
            hasUpdated.UpdatedOnUtc = DateTimeOffset.UtcNow;

        if (auditUserId.HasValue && entry.Entity is IModificationAudited<TUserKey> modificationAudited)
            modificationAudited.LastModifierUserId = auditUserId.Value;
    }

    private static void SetDeletionAudit<TUserKey>(EntityEntry entry, TUserKey? auditUserId)
        where TUserKey : struct
    {
        if (entry.Entity is IHasDeletedOnUtc hasDeleted)
            hasDeleted.DeletedOnUtc = DateTimeOffset.UtcNow;

        if (auditUserId.HasValue && entry.Entity is IDeletionAudited<TUserKey> deletionAudited)
            deletionAudited.DeleterUserId = auditUserId.Value;
    }

    private static void HandleSoftDelete<TUserKey>(EntityEntry entry, TUserKey? auditUserId)
        where TUserKey : struct
    {
        if (entry.Entity is ISoftDelete softDelete)
        {
            entry.State = EntityState.Modified;
            softDelete.IsDeleted = true;
            SetDeletionAudit(entry, auditUserId);
        }
    }
}
