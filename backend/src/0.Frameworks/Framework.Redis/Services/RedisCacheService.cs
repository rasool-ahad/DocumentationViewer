using Framework.Redis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Framework.Redis.Services;

internal sealed class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly RedisConnectionOptions _options;

    public RedisCacheService(
        IConnectionMultiplexer multiplexer,
        RedisConnectionOptions options)
    {
        _options = options;
        _database = multiplexer.GetDatabase(options.Database);
    }

    public async Task<T?> GetDataAsync<T>(string key)
    {
        var serializedData = await _database.StringGetAsync(BuildKey(key));

        if (!serializedData.HasValue)
            return default;

        return JsonConvert.DeserializeObject<T>(serializedData!);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(BuildKey(key));
    }

    public async Task<bool> SetDataAsync<T>(
        string key,
        T value,
        TimeSpan? expirationTime = null)
    {
        var serializedValue = JsonConvert.SerializeObject(value);


        return expirationTime != null
            ? await _database.StringSetAsync(BuildKey(key), serializedValue, expirationTime.Value)
            : await _database.StringSetAsync(BuildKey(key), serializedValue);
    }

    public async Task<bool> RemoveDataAsync(string key)
    {
        return await _database.KeyDeleteAsync(BuildKey(key));
    }

    public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
    {
        var server = GetServer();

        var fullPattern = BuildKey(pattern);

        return server
            .Keys(
                database: _options.Database,
                pattern: fullPattern)
            .Select(x => RemovePrefix(x.ToString()));
    }

    public async Task<long> RemoveAllAsync(IEnumerable<string> keys)
    {
        var redisKeys = keys
            .Select(BuildKey)
            .Select(x => (RedisKey)x)
            .ToArray();

        if (redisKeys.Length == 0)
            return 0;

        return await _database.KeyDeleteAsync(redisKeys);
    }

    public async Task<long> RemoveByPatternAsync(string pattern)
    {
        var keys = await GetDataKeysAsync(pattern);

        if (keys.Length == 0)
            return 0;

        return await _database.KeyDeleteAsync(keys);
    }

    private async Task<RedisKey[]> GetDataKeysAsync(string pattern)
    {
        var server = GetServer();

        return server
            .Keys(
                database: _options.Database,
                pattern: BuildKey(pattern))
            .ToArray();
    }

    private IServer GetServer()
    {
        var endpoint = _options.Host;

        return _database.Multiplexer.GetServer(
            endpoint,
            _options.Port);
    }

    private string BuildKey(string key)
    {
        return $"{_options.KeyPrefix}:{key}";
    }

    private string RemovePrefix(string key)
    {
        if (string.IsNullOrEmpty(_options.KeyPrefix))
            return key;

        return key.StartsWith(_options.KeyPrefix)
            ? key[_options.KeyPrefix.Length..]
            : key;
    }
}
