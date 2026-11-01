using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Contract.Repositories.Commands;

/// <summary>
/// Command repository with int primary key and int user key (most common shortcut).
/// </summary>
public interface IBaseCommandRepository<TEntity> : IBaseCommandRepository<TEntity, int, int>
    where TEntity : IEntity<int>
{
}

/// <summary>
/// Command repository with int primary key and custom user key.
/// </summary>
public interface IBaseCommandRepository<TEntity, TUserKey> : IBaseCommandRepository<TEntity, TUserKey, int>
    where TUserKey : struct
    where TEntity : IEntity<int>
{
}

/// <summary>
/// Base command repository contract — defines write operations for entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TUserKey">The user key type (for auditing).</typeparam>
/// <typeparam name="TPrimaryKey">The primary key type.</typeparam>
public interface IBaseCommandRepository<TEntity, TUserKey, TPrimaryKey>
    where TUserKey : struct
    where TPrimaryKey : struct
    where TEntity : IEntity<TPrimaryKey>
{
    IQueryable<TEntity> Query { get; }

    Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(TUserKey? auditUserId, CancellationToken cancellationToken = default);
}
