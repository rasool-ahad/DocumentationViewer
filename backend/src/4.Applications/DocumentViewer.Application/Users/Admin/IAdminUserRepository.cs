using DocumentViewer.Application.Users.Admin.Queries.List;
using DocumentViewer.Contract.Users.Admin;
using Framework.DDD.Domain.Contract.Paginations.Requests;
using Framework.DDD.Domain.Contract.Paginations.Responses;

namespace DocumentViewer.Application.Users.Admin;

public interface IAdminUserRepository
{
    Task<ListAdminUsersResponse> ListAsync(ListAdminUsersRequest request,
        CancellationToken cancellationToken);
    Task<bool> HasAnotherActiveSuperAdminAsync(int excludingId, CancellationToken cancellationToken);
    Task<IAdminUserTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}

public interface IAdminUserTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
