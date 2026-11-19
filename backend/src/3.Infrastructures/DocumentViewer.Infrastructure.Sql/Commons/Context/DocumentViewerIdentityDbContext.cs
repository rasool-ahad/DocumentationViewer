using DocumentViewer.Domain.Users;
using DocumentViewer.Domain.Documents;
using DocumentViewer.Domain.Navigation;
using Framework.DDD.Infrastructure.Sql.Identity.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DocumentViewer.Infrastructure.Sql.Commons.Context;

public sealed class DocumentViewerIdentityDbContext : BaseIdentityDbContext<User, int>
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentComponent> DocumentComponents => Set<DocumentComponent>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<NavigationItem> NavigationItems => Set<NavigationItem>();

    public DocumentViewerIdentityDbContext(DbContextOptions<DocumentViewerIdentityDbContext> options)
        : base(options, typeof(User).Assembly, typeof(DocumentViewerIdentityDbContext).Assembly)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<long>("DocumentIds", "dbo");
        modelBuilder.HasSequence<long>("ComponentIds", "dbo");
        modelBuilder.HasSequence<long>("VersionIds", "dbo");
        modelBuilder.HasSequence<long>("NavigationIds", "dbo");
    }
}

