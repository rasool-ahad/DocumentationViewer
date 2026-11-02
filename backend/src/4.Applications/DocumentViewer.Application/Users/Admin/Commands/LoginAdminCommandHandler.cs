using DocumentViewer.Contract.Users.Admin.Commands;
using DocumentViewer.Contract.Users.Admin.Services;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Commands;

public sealed class LoginAdminCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtTokenService jwtTokenService)
    : ICommandHandler<LoginAdminCommand, LoginAdminResponse>
{
    public async Task<LoginAdminResponse> Handle(LoginAdminCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserName) || string.IsNullOrWhiteSpace(command.Password))
            throw new ApplicationUnauthorizedException("Invalid credentials.");

        var user = await userManager.FindByNameAsync(command.UserName.Trim());
        if (user is null || !user.IsActive || user.Blocked || user.IsDeleted)
            throw new ApplicationUnauthorizedException("Invalid credentials.");

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded ||
            !(await userManager.IsInRoleAsync(user, AdminRoles.Admin) ||
              await userManager.IsInRoleAsync(user, AdminRoles.SuperAdmin)))
            throw new ApplicationUnauthorizedException("Invalid credentials.");

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.CreateAccessToken(user.Id, user.UserName!, roles.ToArray());
        return new LoginAdminResponse(token.Value, "Bearer", token.ExpiresAtUtc);
    }
}
