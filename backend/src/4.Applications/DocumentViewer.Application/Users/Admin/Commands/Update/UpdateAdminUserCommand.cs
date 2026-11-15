using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.Update;

public sealed record UpdateAdminUserCommand(int Id, UpdateAdminUserRequest Request, int ActorId)
    : ICommand<UpdateAdminUserResponse>;
