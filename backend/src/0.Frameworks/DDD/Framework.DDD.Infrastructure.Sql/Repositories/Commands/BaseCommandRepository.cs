using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Contract.Repositories.Commands;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;

namespace Framework.DDD.Infrastructure.Sql.Repositories.Commands;

/// <summary>
/// Command repository with int primary key and int user key (most common shortcut).
/// </summary>
public class BaseCommandRepository<TEntity> : BaseCommandRepository<TEntity, int, int>, IBaseCommandRepository<TEntity>
    where TEntity : class, IEntity<int>
{
    public BaseCommandRepository(IUnitOfWork unitOfWork, ILogger logger) : base(unitOfWork, logger)
    {
    }
}

/// <summary>
/// Command repository with int primary key and custom user key.
/// </summary>
public class BaseCommandRepository<TEntity, TUserKey> : BaseCommandRepository<TEntity, TUserKey, int>, IBaseCommandRepository<TEntity, TUserKey>
    where TEntity : class, IEntity<int>
    where TUserKey : struct
{
    public BaseCommandRepository(IUnitOfWork<TUserKey> unitOfWork, ILogger logger) : base(unitOfWork, logger)
    {
    }
}

/// <summary>
/// Base command repository implementation — provides CRUD operations via EF Core.
/// </summary>
public class BaseCommandRepository<TEntity, TUserKey, TPrimaryKey> : IBaseCommandRepository<TEntity, TUserKey, TPrimaryKey>
    where TPrimaryKey : struct
    where TEntity : class, IEntity<TPrimaryKey>
    where TUserKey : struct
{
    protected readonly IUnitOfWork<TUserKey> UnitOfWork;
    protected readonly ILogger Logger;
    protected readonly DbSet<TEntity> DbSet;

    public IQueryable<TEntity> Query => DbSet.AsQueryable();

    public BaseCommandRepository(IUnitOfWork<TUserKey> unitOfWork, ILogger logger)
    {
        UnitOfWork = unitOfWork;
        Logger = logger;
        DbSet = unitOfWork.Set<TEntity>();
    }

    public virtual async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return true;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        return true;
    }

    public virtual async Task<bool> RemoveAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            Logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for removal", typeof(TEntity).Name, id);
            return false;
        }

        DbSet.Remove(entity);
        return true;
    }

    public virtual Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.FromResult(true);
    }

    public virtual Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        return Task.FromResult(true);
    }

    public virtual Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        UnitOfWork.RegisterChanged(entity);
        return Task.FromResult(true);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> SaveChangesAsync(TUserKey? auditUserId, CancellationToken cancellationToken = default)
    {
        return await UnitOfWork.SaveChangesAsync(auditUserId, cancellationToken);
    }
}
