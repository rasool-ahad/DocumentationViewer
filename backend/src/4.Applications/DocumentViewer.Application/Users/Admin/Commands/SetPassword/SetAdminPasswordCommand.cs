using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.SetPassword;

public sealed record SetAdminPasswordCommand(int Id, SetAdminPasswordRequest Request, int ActorId)
    : ICommand<SetAdminPasswordResponse>;
