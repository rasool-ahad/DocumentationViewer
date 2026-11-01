using System.Reflection;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Infrastructure.Sql.Extensions;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;

namespace Framework.DDD.Infrastructure.Sql.DbContexts;

/// <summary>
/// Base DbContext with int user key (most common shortcut).
/// </summary>
public class BaseDbContext : BaseDbContext<int>, IUnitOfWork
{
    public BaseDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options, entitiesAssembly, configurationsAssembly)
    {
    }

    public BaseDbContext(DbContextOptions options, ILogger<BaseDbContext> logger, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options, logger, entitiesAssembly, configurationsAssembly)
    {
    }
}

/// <summary>
/// Base DbContext implementing IUnitOfWork with full auditing support.
/// </summary>
/// <typeparam name="TUserKey">The user key type used for auditing.</typeparam>
public class BaseDbContext<TUserKey> : DbContext, IUnitOfWork<TUserKey>
    where TUserKey : struct
{
    private readonly Stack<TransactionScope> _transactionScopes = new();
    private readonly ILogger? _logger;

    public DbContext DbContext => this;
    public override DatabaseFacade Database => base.Database;

    protected Assembly EntitiesAssembly;
    protected Assembly ConfigurationsAssembly;
    protected string DefaultSchema = "dbo";

    #region Constructors

    public BaseDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options)
    {
        EntitiesAssembly = entitiesAssembly;
        ConfigurationsAssembly = configurationsAssembly;
    }

    public BaseDbContext(DbContextOptions options, ILogger logger, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options)
    {
        _logger = logger;
        EntitiesAssembly = entitiesAssembly;
        ConfigurationsAssembly = configurationsAssembly;
    }

    #endregion

    #region OnModelCreating

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings
            .Ignore(CoreEventId.RedundantIndexRemoved,
                CoreEventId.MultipleNavigationProperties,
                CoreEventId.ShadowPropertyCreated));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.RegisterAllEntities(EntitiesAssembly);
        modelBuilder.ApplyConfigurationsFromAssembly(ConfigurationsAssembly);
    }

    #endregion

    #region IUnitOfWork

    public void RegisterUnchanged<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        Entry(entity).State = EntityState.Unchanged;
    }

    public void RegisterChanged<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        Update(entity);
    }

    public void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        Set<TEntity>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync(TUserKey? auditUserId, CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyConcepts(auditUserId);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyConcepts<TUserKey>(default);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesWithTransactionAsync(TUserKey? auditUserId = null, CancellationToken cancellationToken = default)
    {
        await using var dbContextTransaction = await Database.BeginTransactionAsync(cancellationToken);

        try
        {
            ChangeTracker.ApplyConcepts(auditUserId);
            ChangeTracker.AutoDetectChangesEnabled = false;

            var result = await base.SaveChangesAsync(cancellationToken);

            ChangeTracker.AutoDetectChangesEnabled = true;
            await dbContextTransaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = isolationLevel,
            Timeout = TransactionManager.DefaultTimeout
        };

        var scope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        _transactionScopes.Push(scope);
    }

    public void Commit()
    {
        if (_transactionScopes.Count > 0)
        {
            var scope = _transactionScopes.Pop();
            try
            {
                scope.Complete();
            }
            finally
            {
                scope.Dispose();
            }
        }
    }

    public void Rollback()
    {
        if (_transactionScopes.Count > 0)
        {
            var scope = _transactionScopes.Pop();
            scope.Dispose();
        }
    }

    #endregion
}
