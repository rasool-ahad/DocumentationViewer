using DocumentViewer.Domain.Documents;
using DocumentViewer.Domain.Documents.Content;
using DocumentViewer.Domain.Documents.Enums;
using Framework.DDD.Domain.Exceptions;
using Xunit;

namespace DocumentViewer.Domain.UnitTests.Documents;

public class DocumentTests
{
    private static Document NewDocument() => new(1, "Guide", "guide", null);

    [Fact]
    public void New_document_is_draft_and_requires_valid_information()
    {
        var document = NewDocument();
        Assert.Equal(DocumentStatus.Draft, document.Status);
        Assert.Equal(0, document.PublishedVersionNumber);
        Assert.True(document.HasUnpublishedChanges);
        Assert.Throws<DomainException>(() => new Document(0, "Guide", "guide", null));
        Assert.Throws<DomainException>(() => new Document(1, " ", "guide", null));
        Assert.Throws<DomainException>(() => new Document(1, new string('a', 201), "guide", null));
        Assert.Throws<DomainException>(() => new Document(1, "Guide", "Bad-Slug", null));
        Assert.Throws<DomainException>(() => new Document(1, "Guide", "guide\n", null));
        Assert.Throws<DomainException>(() => new Document(1, "Guide", new string('a', 201), null));
    }

    [Fact]
    public void Component_lifecycle_keeps_ids_unique_and_orders_contiguous()
    {
        var document = NewDocument();
        document.AddComponent(10, ComponentType.Heading, new HeadingContent("Intro", 1));
        document.AddComponent(20, ComponentType.Text, new TextContent("Body", TextFormat.Markdown));
        document.AddComponent(30, ComponentType.Divider, new DividerContent(null));
        Assert.Throws<DomainException>(() => document.AddComponent(20, ComponentType.Divider, new DividerContent(null)));
        Assert.Throws<DomainException>(() => document.AddComponent(40, ComponentType.Heading, new TextContent("Wrong", TextFormat.PlainText)));
        Assert.Throws<DomainException>(() => document.UpdateComponent(10, new DividerContent(null)));

        document.UpdateComponent(20, new TextContent("Updated", TextFormat.PlainText));
        document.ReorderComponents([30, 10, 20]);
        Assert.Equal([30L, 10L, 20L], document.Components.Select(x => x.Id));
        Assert.Equal([0, 1, 2], document.Components.Select(x => x.Order));
        document.RemoveComponent(10);
        Assert.Equal([30L, 20L], document.Components.Select(x => x.Id));
        Assert.Equal([0, 1], document.Components.Select(x => x.Order));
        Assert.Throws<DomainException>(() => document.RemoveComponent(10));
    }

    [Fact]
    public void Reorder_requires_every_component_exactly_once()
    {
        var document = NewDocument();
        document.AddComponent(10, ComponentType.Divider, new DividerContent(null));
        document.AddComponent(20, ComponentType.Divider, new DividerContent(null));
        Assert.Throws<DomainException>(() => document.ReorderComponents([10, 10]));
        Assert.Throws<DomainException>(() => document.ReorderComponents([10, 30]));
        Assert.Throws<DomainException>(() => document.ReorderComponents([10]));
        Assert.Throws<DomainException>(() => document.ReorderComponents(null!));
        Assert.Equal([10L, 20L], document.Components.Select(x => x.Id));
    }

    [Fact]
    public void Empty_document_can_accept_an_empty_reorder()
    {
        var document = NewDocument();
        document.ReorderComponents([]);
        Assert.Empty(document.Components);
    }

    [Fact]
    public void Publishing_requires_content_and_increments_version_exactly_once()
    {
        var document = NewDocument();
        Assert.Throws<DomainException>(() => document.GetNextVersionNumber());
        document.AddComponent(10, ComponentType.Divider, new DividerContent(null));
        Assert.Equal(1, document.GetNextVersionNumber());
        Assert.Throws<DomainException>(() => document.MarkAsPublished(2));
        document.MarkAsPublished(1);
        Assert.Equal(DocumentStatus.Published, document.Status);
        Assert.Equal(1, document.PublishedVersionNumber);
        Assert.False(document.HasUnpublishedChanges);

        document.UpdateInformation("Updated guide", "updated-guide", "Description");
        Assert.True(document.HasUnpublishedChanges);
        Assert.Equal(DocumentStatus.Published, document.Status);
        document.MarkAsPublished(2);
        Assert.False(document.HasUnpublishedChanges);
        Assert.Equal(2, document.PublishedVersionNumber);
    }

    [Fact]
    public void Archived_document_rejects_changes_and_publication()
    {
        var document = NewDocument();
        document.AddComponent(10, ComponentType.Divider, new DividerContent(null));
        document.Archive();
        document.Archive();
        Assert.Equal(DocumentStatus.Archived, document.Status);
        Assert.Throws<DomainException>(() => document.UpdateInformation("New", "new", null));
        Assert.Throws<DomainException>(() => document.AddComponent(20, ComponentType.Divider, new DividerContent(null)));
        Assert.Throws<DomainException>(() => document.UpdateComponent(10, new DividerContent("dashed")));
        Assert.Throws<DomainException>(() => document.RemoveComponent(10));
        Assert.Throws<DomainException>(() => document.ReorderComponents([10]));
        Assert.Throws<DomainException>(() => document.MarkAsPublished(1));
    }

    [Fact]
    public void Publication_record_contains_only_utc_metadata()
    {
        var at = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var version = new DocumentVersion(11, 1, 1, 2, at);
        Assert.Equal(1, version.DocumentId);
        Assert.Equal(1, version.VersionNumber);
        Assert.Equal(2, version.PublishedBy);
        Assert.Equal(at, version.PublishedAtUtc);
        Assert.Throws<DomainException>(() => version.Id = 12);
        Assert.Throws<DomainException>(() => new DocumentVersion(11, 1, 1, 2, DateTime.SpecifyKind(at, DateTimeKind.Local)));
        Assert.Throws<DomainException>(() => new DocumentVersion(11, 1, 0, 2, at));
    }
}
