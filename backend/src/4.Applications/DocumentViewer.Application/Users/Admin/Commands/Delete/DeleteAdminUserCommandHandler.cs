using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands.Delete;

public sealed class DeleteAdminUserCommandHandler(UserManager<User> users, IAdminUserRepository repository)
    : ICommandHandler<DeleteAdminUserCommand, DeleteAdminUserResponse>
{
    public async Task<DeleteAdminUserResponse> Handle(DeleteAdminUserCommand command, CancellationToken ct)
    {
        var user = await AdminUserRules.FindAsync(users, command.Id);
        if (command.Id == command.ActorId)
            throw new ApplicationBadRequestException("You cannot delete your own account.");
        if (await users.IsInRoleAsync(user, AdminRoles.SuperAdmin))
            await AdminUserRules.EnsureAnotherActiveSuperAdminAsync(repository, command.Id, ct);
        user.IsDeleted = true;
        user.DeletedOnUtc = DateTimeOffset.UtcNow;
        user.DeleterUserId = command.ActorId;
        user.Deactivate();
        user.UserName = $"deleted-{command.Id}-{Guid.NewGuid():N}";
        user.Email = null;
        AdminUserRules.Ensure(await users.UpdateSecurityStampAsync(user));
        return new DeleteAdminUserResponse(true);
    }
}
