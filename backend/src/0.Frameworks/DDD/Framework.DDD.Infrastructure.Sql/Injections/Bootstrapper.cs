using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Framework.DDD.Infrastructure.Sql.Interceptors;
using Framework.DDD.Infrastructure.Sql.Options;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;

namespace Framework.DDD.Infrastructure.Sql.Injections;

public static class Bootstrapper
{
    /// <summary>
    /// Registers a DbContext as both TContext and IUnitOfWork with SQL Server configuration.
    /// </summary>
    public static IServiceCollection AddSqlDbContext<TContext>(
        this IServiceCollection services,
        Action<SqlDbContextOptions> configure)
        where TContext : DbContexts.BaseDbContext
    {
        return services.AddSqlDbContext<TContext, int>(configure);
    }

    /// <summary>
    /// Registers a DbContext as both TContext and IUnitOfWork{TUserKey} with SQL Server configuration.
    /// </summary>
    public static IServiceCollection AddSqlDbContext<TContext, TUserKey>(
        this IServiceCollection services,
        Action<SqlDbContextOptions> configure)
        where TContext : DbContexts.BaseDbContext<TUserKey>
        where TUserKey : struct
    {
        var options = new SqlDbContextOptions();
        configure(options);

        var connectionString = BuildConnectionString(options);

        // Register second-level cache if enabled
        if (options.UseSecondLevelCache)
        {
            services.AddSecondLevelCache(options.SecondLevelCache);
        }

        services.TryAddScoped<PublishDomainEventsInterceptor>();

        // Register interceptors
        foreach (var (interceptorType, lifetime) in options.Interceptors)
        {
            services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, lifetime));
        }

        // Register DbContext
        services.AddDbContext<TContext>((sp, dbOptions) =>
        {
            dbOptions.UseSqlServer(connectionString);
            dbOptions.AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());

            // Add interceptors from DI
            foreach (var (interceptorType, _) in options.Interceptors)
            {
                var interceptor = sp.GetRequiredService(interceptorType);
                if (interceptor is Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor efInterceptor)
                {
                    dbOptions.AddInterceptors(efInterceptor);
                }
            }

            // Add second-level cache interceptor if enabled
            if (options.UseSecondLevelCache)
            {
                var cacheInterceptor = sp.GetRequiredService<EFCoreSecondLevelCacheInterceptor.SecondLevelCacheInterceptor>();
                dbOptions.AddInterceptors(cacheInterceptor);
            }
        });

        // Register IUnitOfWork pointing to the same TContext
        services.Add(new ServiceDescriptor(
            typeof(IUnitOfWork<TUserKey>),
            sp => sp.GetRequiredService<TContext>(),
            ServiceLifetime.Scoped));

        services.Add(new ServiceDescriptor(
            typeof(IUnitOfWork),
            sp => sp.GetRequiredService<TContext>(),
            ServiceLifetime.Scoped));

        return services;
    }

    private static string BuildConnectionString(SqlDbContextOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            return string.Empty;

        var builder = new SqlConnectionStringBuilder(options.ConnectionString);

        if (!string.IsNullOrWhiteSpace(options.DatabaseName))
            builder.InitialCatalog = options.DatabaseName;

        if (!string.IsNullOrWhiteSpace(options.UserId))
            builder.UserID = options.UserId;

        if (!string.IsNullOrWhiteSpace(options.Password))
            builder.Password = options.Password;

        return builder.ConnectionString;
    }
}
