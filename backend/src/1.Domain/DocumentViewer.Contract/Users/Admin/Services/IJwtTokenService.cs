namespace DocumentViewer.Contract.Users.Admin.Services;

public interface IJwtTokenService
{
    JwtToken CreateAccessToken(int userId, string userName, IReadOnlyCollection<string> roles, string securityStamp);
}

public sealed record JwtToken(string Value, DateTimeOffset ExpiresAtUtc);
