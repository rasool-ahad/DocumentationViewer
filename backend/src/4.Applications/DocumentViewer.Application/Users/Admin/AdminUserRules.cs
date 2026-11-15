using DocumentViewer.Application.Users.Admin.Queries.List;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin;

internal static class AdminUserRules
{
    public static async Task<User> FindAsync(UserManager<User> users, int id)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null || user.IsDeleted ||
            !(await users.IsInRoleAsync(user, AdminRoles.Admin) || await users.IsInRoleAsync(user, AdminRoles.SuperAdmin)))
            throw new ApplicationNotFoundException("Admin user was not found.");
        return user;
    }

    public static async Task<string> RoleAsync(UserManager<User> users, User user) =>
        await users.IsInRoleAsync(user, AdminRoles.SuperAdmin) ? AdminRoles.SuperAdmin : AdminRoles.Admin;

    public static async Task<AdminUserItem> ToItemAsync(UserManager<User> users, User user) =>
        new(user.Id, user.UserName!, user.FirstName, user.LastName, user.Email,
            await RoleAsync(users, user), user.IsActive, user.CreatedOnUtc, user.LastLoggedOnUtc);

    public static async Task EnsureAnotherActiveSuperAdminAsync(IAdminUserRepository repository, int excludingId,
        CancellationToken cancellationToken)
    {
        if (!await repository.HasAnotherActiveSuperAdminAsync(excludingId, cancellationToken))
            throw new ApplicationConflictException("At least one active super admin must remain.");
    }

    public static void ValidateRole(string? role)
    {
        if (role is not (AdminRoles.Admin or AdminRoles.SuperAdmin))
            throw new ApplicationBadRequestException("Role must be Admin or SuperAdmin.");
    }

    public static void ValidateFields(string? userName, string? firstName, string? lastName, string? email)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ApplicationBadRequestException("Username, first name and last name are required.");
        if (userName.Trim().Length > 256 || firstName.Trim().Length > 128 || lastName.Trim().Length > 128 ||
            (email?.Trim().Length ?? 0) > 256)
            throw new ApplicationBadRequestException("One or more fields exceed the allowed length.");
        if (!string.IsNullOrWhiteSpace(email) && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email.Trim()))
            throw new ApplicationBadRequestException("Email address is invalid.");
    }

    public static string? NormalizeEmail(string? email) => string.IsNullOrWhiteSpace(email) ? null : email.Trim();

    public static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(string.Join("; ", result.Errors.Select(error => error.Description)));
    }
}
