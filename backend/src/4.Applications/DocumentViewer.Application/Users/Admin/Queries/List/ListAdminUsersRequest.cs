using Framework.DDD.Domain.Contract.Paginations.Requests;

namespace DocumentViewer.Application.Users.Admin.Queries.List;

public sealed class ListAdminUsersRequest : PaginationRequest<AdminUserFilter> { }
