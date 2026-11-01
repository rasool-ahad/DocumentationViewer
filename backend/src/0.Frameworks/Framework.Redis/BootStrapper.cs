using Framework.Redis.Models;
using Framework.Redis.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Framework.Redis;

public static class BootStrapper
{
    public static IServiceCollection AddRedisFramework(this IServiceCollection services, Action<RedisConnectionOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new RedisConnectionOptions();
        configure(options);

        if (string.IsNullOrWhiteSpace(options.Host))
            throw new ArgumentException(
                "Redis host is required.",
                nameof(configure));

        var configuration = new ConfigurationOptions
        {
            User = options.Username,
            Password = options.Password,
            DefaultDatabase = options.Database,
            AbortOnConnectFail = false,
        };

        configuration.EndPoints.Add(options.Host, options.Port);

        var connectionMultiplexer =
            ConnectionMultiplexer.Connect(configuration);

        services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

        return services;
    }

    public static IServiceCollection AddRedisCacheServicesFramework(this IServiceCollection services)
    {
        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        return services;
    }
}