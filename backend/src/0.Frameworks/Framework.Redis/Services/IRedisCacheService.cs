namespace Framework.Redis.Services;

public interface IRedisCacheService
{
    Task<T?> GetDataAsync<T>(string key);

    Task<bool> KeyExistsAsync(string key);

    Task<bool> SetDataAsync<T>(
        string key,
        T value,
        TimeSpan? expirationTime = null);

    Task<bool> RemoveDataAsync(string key);

    Task<IEnumerable<string>> GetKeysAsync(string pattern);

    Task<long> RemoveAllAsync(IEnumerable<string> keys);

    Task<long> RemoveByPatternAsync(string pattern);
}