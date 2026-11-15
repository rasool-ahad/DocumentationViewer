using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.Logout;

public sealed record LogoutAdminCommand(string? TokenId, string? ExpiryClaim) : ICommand<LogoutAdminResponse>;
