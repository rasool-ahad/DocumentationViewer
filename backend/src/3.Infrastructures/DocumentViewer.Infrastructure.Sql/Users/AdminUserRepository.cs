using DocumentViewer.Application.Users.Admin.Queries.List;
using DocumentViewer.Application.Users.Admin;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Infrastructure.Sql.Commons.Context;
using Framework.DDD.Domain.Contract.Paginations.Enums;
using Framework.DDD.Domain.Contract.Paginations.Requests;
using Framework.DDD.Domain.Contract.Paginations.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DocumentViewer.Infrastructure.Sql.Users;

public sealed class AdminUserRepository(DocumentViewerIdentityDbContext db) : IAdminUserRepository
{
    public async Task<ListAdminUsersResponse> ListAsync(
        ListAdminUsersRequest request, CancellationToken ct)
    {
        var index = request.Paging.Index;
        var size = request.Paging.Size;

        var query = from user in db.Users.AsNoTracking()
                    join userRole in db.UserRoles on user.Id equals userRole.UserId
                    join role in db.Roles on userRole.RoleId equals role.Id
                    where role.Name == AdminRoles.Admin || role.Name == AdminRoles.SuperAdmin
                    select new { user, role.Name };

        if (!string.IsNullOrWhiteSpace(request.Filter.Search))
        {
            var term = request.Filter.Search.Trim();
            query = query.Where(x => x.user.UserName!.Contains(term) || x.user.FirstName.Contains(term) ||
                x.user.LastName.Contains(term) || (x.user.Email != null && x.user.Email.Contains(term)));
        }

        var count = await query.CountAsync(ct);
        var order = request.Orders.FirstOrDefault();
        var descending = order.Value == RequestOrderType.Descending;
        query = order.Key?.ToLowerInvariant() switch
        {
            null or "id" => descending ? query.OrderByDescending(x => x.user.Id) : query.OrderBy(x => x.user.Id),
            "username" => descending ? query.OrderByDescending(x => x.user.UserName).ThenBy(x => x.user.Id)
                : query.OrderBy(x => x.user.UserName).ThenBy(x => x.user.Id),
            "firstname" => descending ? query.OrderByDescending(x => x.user.FirstName).ThenBy(x => x.user.Id)
                : query.OrderBy(x => x.user.FirstName).ThenBy(x => x.user.Id),
            "lastname" => descending ? query.OrderByDescending(x => x.user.LastName).ThenBy(x => x.user.Id)
                : query.OrderBy(x => x.user.LastName).ThenBy(x => x.user.Id),
            "createdonutc" => descending ? query.OrderByDescending(x => x.user.CreatedOnUtc).ThenBy(x => x.user.Id)
                : query.OrderBy(x => x.user.CreatedOnUtc).ThenBy(x => x.user.Id),
            _ => throw new ArgumentOutOfRangeException(nameof(request), "Unsupported admin user order field.")
        };

        var items = await query.Skip(index * size).Take(size)
            .Select(x => new AdminUserItem(x.user.Id, x.user.UserName!, x.user.FirstName, x.user.LastName,
                x.user.Email, x.Name!, x.user.IsActive, x.user.CreatedOnUtc, x.user.LastLoggedOnUtc))
            .ToListAsync(ct);
        return new ListAdminUsersResponse(items, new BasePaginationResult(count, index, size));
    }

    public async Task<bool> HasAnotherActiveSuperAdminAsync(int excludingId, CancellationToken ct)
    {
        return await (from user in db.Users.AsNoTracking()
            join userRole in db.UserRoles on user.Id equals userRole.UserId
            join role in db.Roles on userRole.RoleId equals role.Id
            where role.Name == AdminRoles.SuperAdmin && user.Id != excludingId && user.IsActive && !user.Blocked
            select user.Id).AnyAsync(ct);
    }

    public async Task<IAdminUserTransaction> BeginTransactionAsync(CancellationToken ct) =>
        new AdminUserTransaction(await db.Database.BeginTransactionAsync(ct));

    private sealed class AdminUserTransaction(IDbContextTransaction transaction) : IAdminUserTransaction
    {
        public Task CommitAsync(CancellationToken ct) => transaction.CommitAsync(ct);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
