using DocumentViewer.Domain.Users;
using Framework.DDD.Infrastructure.Sql.Identity.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DocumentViewer.Infrastructure.Sql.Commons.Context;

public sealed class DocumentViewerIdentityDbContext : BaseIdentityDbContext<User, int>
{
    public DocumentViewerIdentityDbContext(DbContextOptions<DocumentViewerIdentityDbContext> options)
        : base(options, typeof(User).Assembly, typeof(DocumentViewerIdentityDbContext).Assembly)
    {
    }
}

