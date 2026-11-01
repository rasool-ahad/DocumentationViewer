using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Contract.Repositories.Queries;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;

namespace Framework.DDD.Infrastructure.Sql.Repositories.Queries;

/// <summary>
/// Query repository with int primary key (most common shortcut).
/// </summary>
public class BaseQueryRepository<TEntity> : BaseQueryRepository<TEntity, int, int>, IBaseQueryRepository<TEntity>
    where TEntity : class, IEntity<int>
{
    public BaseQueryRepository(IUnitOfWork unitOfWork, ILogger logger) : base(unitOfWork, logger)
    {
    }
}

/// <summary>
/// Query repository with int primary key and custom user key.
/// </summary>
public class BaseQueryRepository<TEntity, TUserKey> : BaseQueryRepository<TEntity, TUserKey, int>, IBaseQueryRepository<TEntity>
    where TEntity : class, IEntity<int>
    where TUserKey : struct
{
    public BaseQueryRepository(IUnitOfWork<TUserKey> unitOfWork, ILogger logger) : base(unitOfWork, logger)
    {
    }
}

/// <summary>
/// Base query repository implementation — provides read operations via EF Core.
/// </summary>
public class BaseQueryRepository<TEntity, TUserKey, TPrimaryKey> : IBaseQueryRepository<TEntity, TPrimaryKey>
    where TUserKey : struct
    where TPrimaryKey : struct
    where TEntity : class, IEntity<TPrimaryKey>
{
    protected readonly IUnitOfWork<TUserKey> UnitOfWork;
    protected readonly ILogger Logger;
    protected readonly DbSet<TEntity> DbSet;

    public IQueryable<TEntity> Query => DbSet.AsQueryable();

    public BaseQueryRepository(IUnitOfWork<TUserKey> unitOfWork, ILogger logger)
    {
        UnitOfWork = unitOfWork;
        Logger = logger;
        DbSet = unitOfWork.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}
