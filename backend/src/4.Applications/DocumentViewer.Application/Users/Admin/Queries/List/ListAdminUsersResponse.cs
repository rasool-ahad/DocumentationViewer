using Framework.DDD.Domain.Contract.Paginations.Responses;

namespace DocumentViewer.Application.Users.Admin.Queries.List;

public sealed class ListAdminUsersResponse(List<AdminUserItem> items, BasePaginationResult pagination)
    : PaginationResponse<AdminUserItem>(items, pagination) { }
