using Framework.Redis;
using Framework.Redis.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DocumentViewer.Infrastructure.Redis;

public static class Bootstrapper
{
    public static IServiceCollection AddDocumentViewerRedisInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RedisConnectionOptions>()
            .Bind(configuration.GetSection("Redis"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Host), "Redis:Host is required.")
            .Validate(options => options.Port is > 0 and <= 65535, "Redis:Port must be valid.")
            .ValidateOnStart();

        services.AddSingleton(provider => provider.GetRequiredService<IOptions<RedisConnectionOptions>>().Value);
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redis = provider.GetRequiredService<IOptions<RedisConnectionOptions>>().Value;
            var options = new ConfigurationOptions
            {
                User = redis.Username,
                Password = redis.Password,
                DefaultDatabase = redis.Database,
                AbortOnConnectFail = false
            };
            options.EndPoints.Add(redis.Host, redis.Port);
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddRedisCacheServicesFramework();
        return services;
    }
}

