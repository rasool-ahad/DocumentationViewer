using Framework.DDD.Application.Queries;

namespace DocumentViewer.Application.Users.Admin.Queries.List;

public sealed record ListAdminUsersQuery(ListAdminUsersRequest Request)
    : IQuery<ListAdminUsersResponse>;
