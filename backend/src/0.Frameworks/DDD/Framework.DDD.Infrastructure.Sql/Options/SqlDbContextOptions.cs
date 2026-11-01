namespace Framework.DDD.Infrastructure.Sql.Options;

/// <summary>
/// Options for configuring the SQL DbContext registration.
/// </summary>
public class SqlDbContextOptions
{
    /// <summary>
    /// SQL Server connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database name (overrides the one in the connection string if set).
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Database user (overrides the one in the connection string if set).
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Database password (overrides the one in the connection string if set).
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Additional interceptors to register (Type → ServiceLifetime).
    /// </summary>
    public Dictionary<Type, Microsoft.Extensions.DependencyInjection.ServiceLifetime> Interceptors { get; set; } = new();

    /// <summary>
    /// Whether to enable second-level cache. When true, SecondLevelCacheOptions must be configured.
    /// </summary>
    public bool UseSecondLevelCache { get; set; }

    /// <summary>
    /// Second-level cache options (only used when UseSecondLevelCache = true).
    /// </summary>
    public SecondLevelCacheOptions SecondLevelCache { get; set; } = new();
}
