using DocumentViewer.Domain.Documents.Content;
using DocumentViewer.Domain.Documents.Enums;
using Framework.DDD.Domain.Entities;
using Framework.DDD.Domain.Guards;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Documents;

public sealed class Document : AggregateRoot<long>
{
    private readonly List<DocumentComponent> _components = [];

    public sealed override long Id
    {
        get => base.Id;
        set
        {
            DomainGuard.ThrowIf.NotEqual(value > 0 && (base.Id == 0 || base.Id == value), false,
                "Document ID cannot be changed.");
            base.Id = value;
        }
    }

    public string Title { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public DocumentStatus Status { get; private set; }
    public int PublishedVersionNumber { get; private set; }
    public bool HasUnpublishedChanges { get; private set; }
    public IReadOnlyCollection<DocumentComponent> Components => _components.AsReadOnly();

    private Document() { }

    public Document(long id, string title, string slug, string? description)
    {
        DomainGuard.ThrowIf.GreaterThan(id, 0L, "Document ID must be positive.");
        ValidateTitle(title);
        ValidateSlug(slug);
        Id = id;
        Title = title;
        Slug = slug;
        Description = description;
        Status = DocumentStatus.Draft;
        HasUnpublishedChanges = true;
    }

    public void UpdateInformation(string title, string slug, string? description)
    {
        EnsureNotArchived();
        ValidateTitle(title);
        ValidateSlug(slug);
        Title = title;
        Slug = slug;
        Description = description;
        HasUnpublishedChanges = true;
    }

    public void AddComponent(long componentId, ComponentType type, ComponentContent content)
    {
        EnsureNotArchived();
        DomainGuard.ThrowIf.NotEqual(_components.Any(x => x.Id == componentId), true,
            "Component already exists.");
        var component = new DocumentComponent(componentId, type, _components.Count, content);
        _components.Add(component);
        HasUnpublishedChanges = true;
    }

    public void UpdateComponent(long componentId, ComponentContent content)
    {
        EnsureNotArchived();
        GetComponent(componentId).UpdateContent(content);
        HasUnpublishedChanges = true;
    }

    public void RemoveComponent(long componentId)
    {
        EnsureNotArchived();
        _components.Remove(GetComponent(componentId));
        NormalizeComponentOrder();
        HasUnpublishedChanges = true;
    }

    public void ReorderComponents(IReadOnlyCollection<long> componentIds)
    {
        EnsureNotArchived();
        DomainGuard.ThrowIf.NotEqual(componentIds is null, true, "Component order is required.");
        DomainGuard.ThrowIf.NotEqual(componentIds!.Count != _components.Count ||
            componentIds.Distinct().Count() != _components.Count ||
            !componentIds.ToHashSet().SetEquals(_components.Select(x => x.Id)), true,
            "The supplied order must contain every component exactly once.");

        var byId = _components.ToDictionary(x => x.Id);
        _components.Clear();
        foreach (var id in componentIds)
            _components.Add(byId[id]);
        NormalizeComponentOrder();
        HasUnpublishedChanges = true;
    }

    public int GetNextVersionNumber()
    {
        EnsureNotArchived();
        DomainGuard.ThrowIf.GreaterThan(_components.Count, 0,
            "A document without components cannot be published.");
        DomainGuard.ThrowIf.LessThan(PublishedVersionNumber, int.MaxValue,
            "Document version number cannot be increased.");
        return PublishedVersionNumber + 1;
    }

    // The publication use case must persist DocumentVersion with this state change in one transaction.
    public void MarkAsPublished(int versionNumber)
    {
        DomainGuard.ThrowIf.NotEqual(versionNumber == GetNextVersionNumber(), false,
            "Invalid document version number.");
        PublishedVersionNumber = versionNumber;
        Status = DocumentStatus.Published;
        HasUnpublishedChanges = false;
    }

    public void Archive() => Status = DocumentStatus.Archived;

    private DocumentComponent GetComponent(long componentId)
    {
        var component = _components.FirstOrDefault(x => x.Id == componentId);
        DomainGuard.ThrowIf.NotEmpty(component, "Component not found.");
        return component!;
    }

    private void NormalizeComponentOrder()
    {
        for (var index = 0; index < _components.Count; index++)
            _components[index].ChangeOrder(index);
    }

    private void EnsureNotArchived()
    {
        DomainGuard.ThrowIf.NotEqual(Status, DocumentStatus.Archived,
            "Archived documents cannot be modified.");
    }

    private static void ValidateTitle(string title)
    {
        DomainGuard.ThrowIf.NotEmpty(title, "Document title is required.");
        DomainGuard.ThrowIf.LessThanOrEqual(title.Length, 200,
            "Document title cannot exceed 200 characters.");
    }

    private static void ValidateSlug(string slug)
    {
        DomainGuard.ThrowIf.NotEmpty(slug, "Document slug is required.");
        DomainGuard.ThrowIf.LessThanOrEqual(slug.Length, 200,
            "Document slug cannot exceed 200 characters.");
        DomainGuard.ThrowIf.Match(slug, "\\A[a-z0-9]+(?:-[a-z0-9]+)*\\z", "Invalid slug format.");
    }
}
