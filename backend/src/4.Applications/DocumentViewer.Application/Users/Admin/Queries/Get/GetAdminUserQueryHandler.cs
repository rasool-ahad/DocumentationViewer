using DocumentViewer.Domain.Users;
using Framework.DDD.Application.Queries;
using Microsoft.AspNetCore.Identity;

namespace DocumentViewer.Application.Users.Admin.Queries.Get;

public sealed class GetAdminUserQueryHandler(UserManager<User> users) : IQueryHandler<GetAdminUserQuery, GetAdminUserResponse>
{
    public async Task<GetAdminUserResponse> Handle(GetAdminUserQuery query, CancellationToken ct)
    {
        var item = await AdminUserRules.ToItemAsync(users, await AdminUserRules.FindAsync(users, query.Id));
        return new GetAdminUserResponse(item.Id, item.UserName, item.FirstName, item.LastName,
            item.Email, item.Role, item.IsActive, item.CreatedOnUtc, item.LastLoggedOnUtc);
    }
}
