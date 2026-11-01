using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;
using Framework.DDD.Infrastructure.Sql.Options;
using StackExchange.Redis;

namespace Framework.DDD.Infrastructure.Sql.Injections;

internal static class SecondLevelCacheBootstrapper
{
    /// <summary>
    /// Registers EFCoreSecondLevelCacheInterceptor with Redis backing store.
    /// </summary>
    public static IServiceCollection AddSecondLevelCache(
        this IServiceCollection services,
        SecondLevelCacheOptions options)
    {
        services.AddEFSecondLevelCache(cacheOptions =>
        {
            cacheOptions.UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1));

            if (!string.IsNullOrWhiteSpace(options.CacheKeyPrefix))
            {
                cacheOptions.UseCacheKeyPrefix(options.CacheKeyPrefix);
            }

            // Configure Redis as the cache provider
            if (options.RedisConfigurationAction is not null)
            {
                cacheOptions.UseEasyCachingCoreProvider("Redis", isHybridCache: false)
                    .ConfigureLogging(true);
            }
            else if (!string.IsNullOrWhiteSpace(options.RedisConnectionString))
            {
                cacheOptions.UseEasyCachingCoreProvider("Redis", isHybridCache: false)
                    .ConfigureLogging(true);
            }
        });

        // Register EasyCaching with Redis
        services.AddEasyCaching(cachingOptions =>
        {
            cachingOptions.UseRedis(redisConfig =>
            {
                if (options.RedisConfigurationAction is not null)
                {
                    var configOptions = new ConfigurationOptions();
                    options.RedisConfigurationAction(configOptions);

                    foreach (var endpoint in configOptions.EndPoints)
                    {
                        if (endpoint is System.Net.DnsEndPoint dnsEndPoint)
                        {
                            redisConfig.DBConfig.Endpoints.Add(
                                new EasyCaching.Core.Configurations.ServerEndPoint(dnsEndPoint.Host, dnsEndPoint.Port));
                        }
                        else if (endpoint is System.Net.IPEndPoint ipEndPoint)
                        {
                            redisConfig.DBConfig.Endpoints.Add(
                                new EasyCaching.Core.Configurations.ServerEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(configOptions.Password))
                        redisConfig.DBConfig.Password = configOptions.Password;

                    if (configOptions.DefaultDatabase.HasValue)
                        redisConfig.DBConfig.Database = configOptions.DefaultDatabase.Value;

                    redisConfig.DBConfig.IsSsl = configOptions.Ssl;
                }
                else if (!string.IsNullOrWhiteSpace(options.RedisConnectionString))
                {
                    var parsed = ConfigurationOptions.Parse(options.RedisConnectionString);

                    foreach (var endpoint in parsed.EndPoints)
                    {
                        if (endpoint is System.Net.DnsEndPoint dnsEndPoint)
                        {
                            redisConfig.DBConfig.Endpoints.Add(
                                new EasyCaching.Core.Configurations.ServerEndPoint(dnsEndPoint.Host, dnsEndPoint.Port));
                        }
                        else if (endpoint is System.Net.IPEndPoint ipEndPoint)
                        {
                            redisConfig.DBConfig.Endpoints.Add(
                                new EasyCaching.Core.Configurations.ServerEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(parsed.Password))
                        redisConfig.DBConfig.Password = parsed.Password;

                    if (parsed.DefaultDatabase.HasValue)
                        redisConfig.DBConfig.Database = parsed.DefaultDatabase.Value;

                    redisConfig.DBConfig.IsSsl = parsed.Ssl;
                }
            }, "Redis");
        });

        return services;
    }
}
