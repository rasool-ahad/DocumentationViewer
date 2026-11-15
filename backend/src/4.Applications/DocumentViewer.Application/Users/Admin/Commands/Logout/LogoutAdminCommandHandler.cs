using DocumentViewer.Contract.Users.Admin.Services;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;

namespace DocumentViewer.Application.Users.Admin.Commands.Logout;

public sealed class LogoutAdminCommandHandler(IAccessTokenRevocationService revocations)
    : ICommandHandler<LogoutAdminCommand, LogoutAdminResponse>
{
    public async Task<LogoutAdminResponse> Handle(LogoutAdminCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.TokenId) ||
            !long.TryParse(command.ExpiryClaim, out var expiryUnixSeconds) ||
            expiryUnixSeconds < DateTimeOffset.MinValue.ToUnixTimeSeconds() ||
            expiryUnixSeconds > DateTimeOffset.MaxValue.ToUnixTimeSeconds())
            throw new ApplicationUnauthorizedException("Invalid access token.");

        await revocations.RevokeAsync(command.TokenId, DateTimeOffset.FromUnixTimeSeconds(expiryUnixSeconds));
        return new LogoutAdminResponse(true);
    }
}
