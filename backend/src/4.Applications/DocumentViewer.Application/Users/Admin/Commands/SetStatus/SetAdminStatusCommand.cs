using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.SetStatus;

public sealed record SetAdminStatusCommand(int Id, SetAdminStatusRequest Request, int ActorId)
    : ICommand<SetAdminStatusResponse>;
