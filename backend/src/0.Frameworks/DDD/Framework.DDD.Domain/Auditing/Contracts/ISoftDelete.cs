namespace Framework.DDD.Domain.Auditing.Contracts;

/// <summary>
/// Marks an entity as soft-deletable.
/// Soft-deleted entities are not physically removed; they are flagged with IsDeleted = true.
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
