using System.Linq.Expressions;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Domain.Contract.Repositories.Queries;

/// <summary>
/// Query repository with int primary key (most common shortcut).
/// </summary>
public interface IBaseQueryRepository<TEntity> : IBaseQueryRepository<TEntity, int>
    where TEntity : IEntity<int>
{
}

/// <summary>
/// Base query repository contract — defines read operations for entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TPrimaryKey">The primary key type.</typeparam>
public interface IBaseQueryRepository<TEntity, TPrimaryKey>
    where TPrimaryKey : struct
    where TEntity : IEntity<TPrimaryKey>
{
    IQueryable<TEntity> Query { get; }

    Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
