namespace Framework.Redis.Models;

public class RedisConnectionOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 6379;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int Database { get; set; } = 0;

    public string? KeyPrefix { get; set; }
}
