using DocumentViewer.Contract.Users.Admin.Commands;
using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands;

public sealed class LoginAdminCommand(string userName, string password) : ICommand<LoginAdminResponse>
{
    public string UserName { get; } = userName;
    public string Password { get; } = password;
}
