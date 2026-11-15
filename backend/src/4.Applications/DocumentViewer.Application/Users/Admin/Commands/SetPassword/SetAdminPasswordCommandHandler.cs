using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands.SetPassword;

public sealed class SetAdminPasswordCommandHandler(UserManager<User> users, IAdminUserRepository repository)
    : ICommandHandler<SetAdminPasswordCommand, SetAdminPasswordResponse>
{
    public async Task<SetAdminPasswordResponse> Handle(SetAdminPasswordCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Request.Password))
            throw new ApplicationBadRequestException("Password is required.");
        var user = await AdminUserRules.FindAsync(users, command.Id);
        await using var transaction = await repository.BeginTransactionAsync(ct);
        if (await users.HasPasswordAsync(user)) AdminUserRules.Ensure(await users.RemovePasswordAsync(user));
        AdminUserRules.Ensure(await users.AddPasswordAsync(user, command.Request.Password));
        user.UpdatedOnUtc = DateTimeOffset.UtcNow;
        user.LastModifierUserId = command.ActorId;
        AdminUserRules.Ensure(await users.UpdateSecurityStampAsync(user));
        await transaction.CommitAsync(ct);
        return new SetAdminPasswordResponse(true);
    }
}
