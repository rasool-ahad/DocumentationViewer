namespace Framework.DDD.Domain.Entities;

/// <summary>
/// Marker interface for all domain entities.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Interface for entities with a single-value primary key.
/// </summary>
/// <typeparam name="TPrimaryKey">Type of the primary key.</typeparam>
public interface IEntity<TPrimaryKey> : IEntity where TPrimaryKey : struct
{
    /// <summary>Unique identifier for this entity.</summary>
    TPrimaryKey Id { get; }

    /// <summary>Returns true when the entity has not yet been persisted (Id is default value).</summary>
    bool IsTransient();
}
