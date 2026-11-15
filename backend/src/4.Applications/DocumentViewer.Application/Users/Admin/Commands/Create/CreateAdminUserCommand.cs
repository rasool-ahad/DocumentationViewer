using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.Create;

public sealed record CreateAdminUserCommand(CreateAdminUserRequest Request, int ActorId) : ICommand<CreateAdminUserResponse>;
