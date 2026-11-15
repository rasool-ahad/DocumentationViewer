using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands.SetStatus;

public sealed class SetAdminStatusCommandHandler(UserManager<User> users, IAdminUserRepository repository)
    : ICommandHandler<SetAdminStatusCommand, SetAdminStatusResponse>
{
    public async Task<SetAdminStatusResponse> Handle(SetAdminStatusCommand command, CancellationToken ct)
    {
        var user = await AdminUserRules.FindAsync(users, command.Id);
        if (!command.Request.IsActive && command.Id == command.ActorId)
            throw new ApplicationBadRequestException("You cannot deactivate your own account.");
        if (!command.Request.IsActive && await users.IsInRoleAsync(user, AdminRoles.SuperAdmin))
            await AdminUserRules.EnsureAnotherActiveSuperAdminAsync(repository, command.Id, ct);
        if (command.Request.IsActive) user.Activate(); else user.Deactivate();
        user.UpdatedOnUtc = DateTimeOffset.UtcNow;
        user.LastModifierUserId = command.ActorId;
        AdminUserRules.Ensure(await users.UpdateSecurityStampAsync(user));
        return new SetAdminStatusResponse(true);
    }
}
