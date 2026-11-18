using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Guards;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Navigation;

public sealed class NavigationItem : AggregateRoot<long>
{
    public sealed override long Id
    {
        get => base.Id;
        set
        {
            DomainGuard.ThrowIf.NotEqual(value > 0 && (base.Id == 0 || base.Id == value), false,
                "Navigation item ID cannot be changed.");
            base.Id = value;
        }
    }

    public long? ParentId { get; private set; }
    public long? DocumentId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Slug { get; private set; }
    public string? Icon { get; private set; }
    public string? ExternalUrl { get; private set; }
    public NavigationItemType Type { get; private set; }
    public int Order { get; private set; }
    public bool IsVisible { get; private set; }

    private NavigationItem() { }

    public NavigationItem(long id, string title, NavigationItemType type, int order,
        long? parentId = null, long? documentId = null, string? externalUrl = null,
        string? slug = null, string? icon = null)
    {
        DomainGuard.ThrowIf.GreaterThan(id, 0L, "Navigation item ID must be positive.");
        ValidateTitle(title);
        ValidateParentId(parentId, id);
        DomainGuard.ThrowIf.GreaterThanOrEqual(order, 0, "Navigation order cannot be negative.");
        ValidateTypeConfiguration(type, documentId, externalUrl);

        Id = id;
        Title = title;
        Type = type;
        Order = order;
        ParentId = parentId;
        DocumentId = documentId;
        ExternalUrl = externalUrl;
        Slug = slug;
        Icon = icon;
        IsVisible = true;
    }

    public void UpdateTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
    }

    public void UpdateDisplay(string? slug, string? icon)
    {
        Slug = slug;
        Icon = icon;
    }

    // The caller must validate the complete tree before assigning a non-null parent.
    public void ChangeParent(long? parentId)
    {
        ValidateParentId(parentId, Id);
        ParentId = parentId;
    }

    public void ChangeOrder(int order)
    {
        DomainGuard.ThrowIf.GreaterThanOrEqual(order, 0, "Navigation order cannot be negative.");
        Order = order;
    }

    public void SetVisibility(bool isVisible) => IsVisible = isVisible;

    public void UpdateExternalUrl(string externalUrl)
    {
        DomainGuard.ThrowIf.NotEqual(Type == NavigationItemType.ExternalLink, false,
            "Only external links can update an external URL.");
        DomainGuard.ThrowIf.NotEmpty(externalUrl, "External URL is required.");
        ExternalUrl = externalUrl;
    }

    private static void ValidateParentId(long? parentId, long id)
    {
        if (parentId is not null)
        {
            DomainGuard.ThrowIf.GreaterThan(parentId.Value, 0L, "Parent ID must be positive.");
            DomainGuard.ThrowIf.NotEqual(parentId.Value, id, "An item cannot be its own parent.");
        }
    }

    private static void ValidateTypeConfiguration(NavigationItemType type, long? documentId, string? externalUrl)
    {
        var valid = type switch
        {
            NavigationItemType.Group => documentId is null && externalUrl is null,
            NavigationItemType.Document => documentId > 0 && externalUrl is null,
            NavigationItemType.ExternalLink => documentId is null && !string.IsNullOrWhiteSpace(externalUrl),
            _ => false
        };
        DomainGuard.ThrowIf.NotEqual(valid, false, "Invalid navigation item type configuration.");
    }

    private static void ValidateTitle(string title)
    {
        DomainGuard.ThrowIf.NotEmpty(title, "Navigation title is required.");
        DomainGuard.ThrowIf.LessThanOrEqual(title.Length, 200,
            "Navigation title cannot exceed 200 characters.");
    }
}
