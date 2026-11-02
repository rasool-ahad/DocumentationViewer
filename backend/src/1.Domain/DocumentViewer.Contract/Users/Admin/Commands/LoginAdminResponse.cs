namespace DocumentViewer.Contract.Users.Admin.Commands;

public sealed record LoginAdminResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc);
