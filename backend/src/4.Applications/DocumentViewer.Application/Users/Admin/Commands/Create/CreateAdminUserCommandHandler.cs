using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands.Create;

public sealed class CreateAdminUserCommandHandler(
    UserManager<User> users, RoleManager<BaseRole<int>> roles, IAdminUserRepository repository)
    : ICommandHandler<CreateAdminUserCommand, CreateAdminUserResponse>
{
    public async Task<CreateAdminUserResponse> Handle(CreateAdminUserCommand command, CancellationToken ct)
    {
        var request = command.Request;
        AdminUserRules.ValidateRole(request.Role);
        AdminUserRules.ValidateFields(request.UserName, request.FirstName, request.LastName, request.Email);
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ApplicationBadRequestException("Password is required.");

        await using var transaction = await repository.BeginTransactionAsync(ct);
        if (!await roles.RoleExistsAsync(request.Role))
            AdminUserRules.Ensure(await roles.CreateAsync(new BaseRole<int> { Name = request.Role }));

        var user = User.CreateBuilder().WithUserName(request.UserName).WithFirstName(request.FirstName)
            .WithLastName(request.LastName).Build();
        user.Email = AdminUserRules.NormalizeEmail(request.Email);
        user.CreatedOnUtc = DateTimeOffset.UtcNow;
        AdminUserRules.Ensure(await users.CreateAsync(user, request.Password));
        AdminUserRules.Ensure(await users.AddToRoleAsync(user, request.Role));
        await transaction.CommitAsync(ct);
        var item = await AdminUserRules.ToItemAsync(users, user);
        return new CreateAdminUserResponse(item.Id, item.UserName, item.FirstName, item.LastName,
            item.Email, item.Role, item.IsActive, item.CreatedOnUtc, item.LastLoggedOnUtc);
    }
}
