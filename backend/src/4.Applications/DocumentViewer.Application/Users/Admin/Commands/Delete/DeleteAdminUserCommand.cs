using Framework.DDD.Application.Commands;

namespace DocumentViewer.Application.Users.Admin.Commands.Delete;

public sealed record DeleteAdminUserCommand(int Id, int ActorId) : ICommand<DeleteAdminUserResponse>;
