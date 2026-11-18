using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Guards;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Documents;

// Publication metadata only; published content is not snapshotted.
public sealed class DocumentVersion : Entity<long>
{
    public sealed override long Id
    {
        get => base.Id;
        set
        {
            DomainGuard.ThrowIf.NotEqual(value > 0 && (base.Id == 0 || base.Id == value), false,
                "Version ID cannot be changed.");
            base.Id = value;
        }
    }

    public long DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public long PublishedBy { get; private set; }
    public DateTime PublishedAtUtc { get; private set; }

    private DocumentVersion() { }

    public DocumentVersion(long id, long documentId, int versionNumber, long publishedBy, DateTime publishedAtUtc)
    {
        DomainGuard.ThrowIf.GreaterThan(id, 0L, "Version ID must be positive.");
        DomainGuard.ThrowIf.GreaterThan(documentId, 0L, "Document ID must be positive.");
        DomainGuard.ThrowIf.GreaterThan(versionNumber, 0, "Version number must be positive.");
        DomainGuard.ThrowIf.GreaterThan(publishedBy, 0L, "Publisher ID must be positive.");
        DomainGuard.ThrowIf.NotEqual(publishedAtUtc != default && publishedAtUtc.Kind == DateTimeKind.Utc,
            false, "Publication date must be UTC.");
        Id = id;
        DocumentId = documentId;
        VersionNumber = versionNumber;
        PublishedBy = publishedBy;
        PublishedAtUtc = publishedAtUtc;
    }
}
