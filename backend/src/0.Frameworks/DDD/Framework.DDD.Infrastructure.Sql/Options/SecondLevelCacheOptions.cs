using StackExchange.Redis;

namespace Framework.DDD.Infrastructure.Sql.Options;

/// <summary>
/// Options for EF Core second-level cache backed by Redis.
/// </summary>
public class SecondLevelCacheOptions
{
    /// <summary>
    /// Cache expiration timeout. Defaults to 30 minutes.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Redis connection string (e.g. "localhost:6379").
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Optional: configure Redis via ConfigurationOptions for advanced scenarios (SSL, user, password, endpoints).
    /// When set, overrides RedisConnectionString.
    /// </summary>
    public Action<ConfigurationOptions>? RedisConfigurationAction { get; set; }

    /// <summary>
    /// Cache key prefix to isolate this app's cache entries.
    /// </summary>
    public string CacheKeyPrefix { get; set; } = string.Empty;
}
