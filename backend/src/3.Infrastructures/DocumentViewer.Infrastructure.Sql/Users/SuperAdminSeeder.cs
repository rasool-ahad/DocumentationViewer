using DocumentViewer.Contract.Options;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Domain.Users;
using DocumentViewer.Infrastructure.Sql.Commons.Context;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DocumentViewer.Infrastructure.Sql.Users;

public static class SuperAdminSeeder
{
    public static async Task SeedSuperAdminAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DocumentViewerIdentityDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<BaseRole<int>>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<SuperAdminOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.UserName))
            throw new InvalidOperationException("SuperAdmin:UserName is required.");

        var existingUser = await userManager.FindByNameAsync(options.UserName);
        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, AdminRoles.SuperAdmin))
                throw new InvalidOperationException("The configured super admin user exists without the SuperAdmin role.");
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException("Set SuperAdmin:Password to seed the initial super admin user.");

        await using var transaction = await db.Database.BeginTransactionAsync();
        if (!await roleManager.RoleExistsAsync(AdminRoles.SuperAdmin))
        {
            var roleResult = await roleManager.CreateAsync(new BaseRole<int> { Name = AdminRoles.SuperAdmin });
            EnsureSucceeded(roleResult, "create the SuperAdmin role");
        }

        var user = User.CreateBuilder()
            .WithUserName(options.UserName)
            .WithFirstName(options.FirstName)
            .WithLastName(options.LastName)
            .Build();
        var createResult = await userManager.CreateAsync(user, options.Password);
        EnsureSucceeded(createResult, "create the super admin user");

        var addRoleResult = await userManager.AddToRoleAsync(user, AdminRoles.SuperAdmin);
        EnsureSucceeded(addRoleResult, "assign the SuperAdmin role");
        await transaction.CommitAsync();
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to {action}: {string.Join("; ", result.Errors.Select(error => error.Description))}");
    }
}
