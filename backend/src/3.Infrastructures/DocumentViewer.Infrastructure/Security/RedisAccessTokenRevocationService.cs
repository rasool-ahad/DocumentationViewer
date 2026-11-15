using DocumentViewer.Contract.Users.Admin.Services;
using Framework.Redis.Services;

namespace DocumentViewer.Infrastructure.Security;

public sealed class RedisAccessTokenRevocationService(IRedisCacheService cache) : IAccessTokenRevocationService
{
    private const string KeyPrefix = "revoked-access-token:";

    public async Task RevokeAsync(string tokenId, DateTimeOffset expiresAtUtc)
    {
        var remaining = expiresAtUtc - DateTimeOffset.UtcNow;
        if (remaining <= TimeSpan.Zero)
            return;

        if (!await cache.SetDataAsync(KeyPrefix + tokenId, true, remaining))
            throw new InvalidOperationException("The access token could not be revoked.");
    }

    public Task<bool> IsRevokedAsync(string tokenId) => cache.KeyExistsAsync(KeyPrefix + tokenId);
}
