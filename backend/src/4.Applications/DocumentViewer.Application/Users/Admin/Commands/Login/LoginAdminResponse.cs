namespace DocumentViewer.Application.Users.Admin.Commands.Login;

public sealed record LoginAdminResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc);
