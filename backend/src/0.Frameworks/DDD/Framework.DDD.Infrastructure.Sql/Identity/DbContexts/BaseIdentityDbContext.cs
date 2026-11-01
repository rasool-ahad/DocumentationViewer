using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Framework.DDD.Infrastructure.Sql.Extensions;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;
using System.Transactions;

namespace Framework.DDD.Infrastructure.Sql.Identity.DbContexts;

/// <summary>
/// Base Identity DbContext with int key (most common shortcut).
/// Uses default identity entity types.
/// </summary>
public class BaseIdentityDbContext : BaseIdentityDbContext<BaseUser, BaseRole, int>
{
    public BaseIdentityDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options, entitiesAssembly, configurationsAssembly)
    {
    }
}

/// <summary>
/// Base Identity DbContext with custom User.
/// </summary>
public class BaseIdentityDbContext<TUser, TKey>
    : BaseIdentityDbContext<TUser, BaseRole<TKey>, TKey, BaseUserClaim<TKey>, BaseUserRole<TKey>, BaseUserLogin<TKey>, BaseRoleClaim<TKey>, BaseUserToken<TKey>>
    where TUser : BaseUser<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public BaseIdentityDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options, entitiesAssembly, configurationsAssembly)
    {
    }
}

/// <summary>
/// Base Identity DbContext with custom User and Role types.
/// </summary>
public class BaseIdentityDbContext<TUser, TRole, TKey>
    : BaseIdentityDbContext<TUser, TRole, TKey, BaseUserClaim<TKey>, BaseUserRole<TKey>, BaseUserLogin<TKey>, BaseRoleClaim<TKey>, BaseUserToken<TKey>>
    where TUser : BaseUser<TKey>
    where TRole : BaseRole<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public BaseIdentityDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options, entitiesAssembly, configurationsAssembly)
    {
    }
}

/// <summary>
/// Base Identity DbContext implementing IUnitOfWork with full auditing support.
/// Extends IdentityDbContext with all identity entity type parameters.
/// </summary>
public class BaseIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>,
      IUnitOfWork<TKey>
    where TUser : BaseUser<TKey>
    where TRole : BaseRole<TKey>
    where TKey : struct, IEquatable<TKey>
    where TUserClaim : BaseUserClaim<TKey>
    where TUserRole : BaseUserRole<TKey>
    where TUserLogin : BaseUserLogin<TKey>
    where TRoleClaim : BaseRoleClaim<TKey>
    where TUserToken : BaseUserToken<TKey>
{
    private readonly Stack<TransactionScope> _transactionScopes = new();

    public DbContext DbContext => this;
    public override DatabaseFacade Database => base.Database;

    protected Assembly EntitiesAssembly;
    protected Assembly ConfigurationsAssembly;
    protected string DefaultSchema = "dbo";

    #region Constructors

    public BaseIdentityDbContext(DbContextOptions options, Assembly entitiesAssembly, Assembly configurationsAssembly)
        : base(options)
    {
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

    public async Task<int> SaveChangesAsync(TKey? auditUserId, CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyConcepts(auditUserId);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyConcepts<TKey>(default);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesWithTransactionAsync(TKey? auditUserId = null, CancellationToken cancellationToken = default)
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
