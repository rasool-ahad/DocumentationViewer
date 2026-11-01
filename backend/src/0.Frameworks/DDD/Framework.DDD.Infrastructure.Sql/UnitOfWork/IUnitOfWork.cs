using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Framework.DDD.Domain.Entities;

namespace Framework.DDD.Infrastructure.Sql.UnitOfWork;

/// <summary>
/// Unit of work interface with int user key (most common shortcut).
/// </summary>
public interface IUnitOfWork : IUnitOfWork<int>
{
}

/// <summary>
/// Unit of work interface — wraps DbContext for transactional operations.
/// </summary>
/// <typeparam name="TUserKey">The user key type used for auditing.</typeparam>
public interface IUnitOfWork<TUserKey> : IDisposable
    where TUserKey : struct
{
    DbContext DbContext { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    void RegisterUnchanged<TEntity>(TEntity entity) where TEntity : class, IEntity;

    void RegisterChanged<TEntity>(TEntity entity) where TEntity : class, IEntity;

    void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class, IEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(TUserKey? auditUserId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesWithTransactionAsync(TUserKey? auditUserId = null, CancellationToken cancellationToken = default);

    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    void Commit();

    void Rollback();
}
