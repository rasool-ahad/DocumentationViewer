using Framework.DDD.Application.Exceptions;
using Framework.DDD.Application.Queries;

namespace DocumentViewer.Application.Users.Admin.Queries.List;

public sealed class ListAdminUsersQueryHandler(IAdminUserRepository repository)
    : IQueryHandler<ListAdminUsersQuery, ListAdminUsersResponse>
{
    public Task<ListAdminUsersResponse> Handle(ListAdminUsersQuery query, CancellationToken ct)
    {
        var request = query.Request;
        var index = request.Paging.Index;
        var size = request.Paging.Size;
        if (index < 0 || size is < 1 or > 100 || (long)index * size > int.MaxValue)
            throw new ApplicationBadRequestException("Page index must be non-negative and page size must be between 1 and 100.");
        if (request.Orders.Count > 1 || request.Orders.Values.Any(order => !Enum.IsDefined(order)) ||
            request.Orders.Keys.Any(field => field.ToLowerInvariant() is not
                ("id" or "username" or "firstname" or "lastname" or "createdonutc")))
            throw new ApplicationBadRequestException("Admin user ordering must use one supported direction and field.");

        return repository.ListAsync(request, ct);
    }
}
