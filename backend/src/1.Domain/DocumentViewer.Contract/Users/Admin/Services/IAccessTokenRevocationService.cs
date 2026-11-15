namespace DocumentViewer.Contract.Users.Admin.Services;

public interface IAccessTokenRevocationService
{
    Task RevokeAsync(string tokenId, DateTimeOffset expiresAtUtc);

    Task<bool> IsRevokedAsync(string tokenId);
}
