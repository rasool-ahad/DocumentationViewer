using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands.Update;

public sealed class UpdateAdminUserCommandHandler(
    UserManager<User> users, RoleManager<BaseRole<int>> roles, IAdminUserRepository repository)
    : ICommandHandler<UpdateAdminUserCommand, UpdateAdminUserResponse>
{
    public async Task<UpdateAdminUserResponse> Handle(UpdateAdminUserCommand command, CancellationToken ct)
    {
        var request = command.Request;
        AdminUserRules.ValidateRole(request.Role);
        AdminUserRules.ValidateFields(request.UserName, request.FirstName, request.LastName, request.Email);
        var user = await AdminUserRules.FindAsync(users, command.Id);
        var previousRole = await AdminUserRules.RoleAsync(users, user);
        if (command.Id == command.ActorId && previousRole != request.Role)
            throw new ApplicationBadRequestException("You cannot change your own role.");
        if (previousRole == AdminRoles.SuperAdmin && request.Role != AdminRoles.SuperAdmin)
            await AdminUserRules.EnsureAnotherActiveSuperAdminAsync(repository, command.Id, ct);

        await using var transaction = await repository.BeginTransactionAsync(ct);
        if (!await roles.RoleExistsAsync(request.Role))
            AdminUserRules.Ensure(await roles.CreateAsync(new BaseRole<int> { Name = request.Role }));
        user.ChangeUserName(request.UserName);
        user.ChangeName(request.FirstName, request.LastName);
        user.Email = AdminUserRules.NormalizeEmail(request.Email);
        user.UpdatedOnUtc = DateTimeOffset.UtcNow;
        user.LastModifierUserId = command.ActorId;
        AdminUserRules.Ensure(await users.UpdateAsync(user));
        if (previousRole != request.Role)
        {
            AdminUserRules.Ensure(await users.RemoveFromRoleAsync(user, previousRole));
            AdminUserRules.Ensure(await users.AddToRoleAsync(user, request.Role));
        }
        AdminUserRules.Ensure(await users.UpdateSecurityStampAsync(user));
        await transaction.CommitAsync(ct);
        var item = await AdminUserRules.ToItemAsync(users, user);
        return new UpdateAdminUserResponse(item.Id, item.UserName, item.FirstName, item.LastName,
            item.Email, item.Role, item.IsActive, item.CreatedOnUtc, item.LastLoggedOnUtc);
    }
}
