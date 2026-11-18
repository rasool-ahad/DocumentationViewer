using DocumentViewer.Domain.Documents.Content;
using DocumentViewer.Domain.Documents.Enums;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Guards;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Documents;

public sealed class DocumentComponent : Entity<long>
{
    public sealed override long Id
    {
        get => base.Id;
        set
        {
            DomainGuard.ThrowIf.NotEqual(value > 0 && (base.Id == 0 || base.Id == value), false,
                "Component ID cannot be changed.");
            base.Id = value;
        }
    }

    public ComponentType Type { get; private set; }
    public int Order { get; private set; }
    public ComponentContent Content { get; private set; } = default!;

    private DocumentComponent() { }

    internal DocumentComponent(long id, ComponentType type, int order, ComponentContent content)
    {
        DomainGuard.ThrowIf.GreaterThan(id, 0L, "Component ID must be positive.");
        DomainGuard.ThrowIf.GreaterThanOrEqual(order, 0, "Component order cannot be negative.");
        ValidateContentType(type, content);
        Id = id;
        Type = type;
        Order = order;
        Content = content;
    }

    internal void UpdateContent(ComponentContent content)
    {
        ValidateContentType(Type, content);
        Content = content;
    }

    internal void ChangeOrder(int order)
    {
        DomainGuard.ThrowIf.GreaterThanOrEqual(order, 0, "Component order cannot be negative.");
        Order = order;
    }

    private static void ValidateContentType(ComponentType type, ComponentContent content)
    {
        DomainGuard.ThrowIf.NotEmpty(content, "Component content is required.");
        var valid = (type, content) switch
        {
            (ComponentType.Heading, HeadingContent) or
            (ComponentType.Text, TextContent) or
            (ComponentType.Code, CodeContent) or
            (ComponentType.Image, ImageContent) or
            (ComponentType.Table, TableContent) or
            (ComponentType.Alert, AlertContent) or
            (ComponentType.Quote, QuoteContent) or
            (ComponentType.List, ListContent) or
            (ComponentType.Video, VideoContent) or
            (ComponentType.Link, LinkContent) or
            (ComponentType.Divider, DividerContent) or
            (ComponentType.File, FileContent) or
            (ComponentType.Embed, EmbedContent) => true,
            _ => false
        };
        DomainGuard.ThrowIf.NotEqual(valid, false, "Component type does not match its content model.");
    }
}
