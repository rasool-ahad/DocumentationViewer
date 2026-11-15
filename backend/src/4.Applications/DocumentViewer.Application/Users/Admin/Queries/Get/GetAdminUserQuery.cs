using Framework.DDD.Application.Queries;

namespace DocumentViewer.Application.Users.Admin.Queries.Get;

public sealed record GetAdminUserQuery(int Id) : IQuery<GetAdminUserResponse>;
