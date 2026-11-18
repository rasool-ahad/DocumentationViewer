using DocumentViewer.Domain.Navigation;
using Framework.DDD.Domain.Exceptions;
using Xunit;

namespace DocumentViewer.Domain.UnitTests.Navigation;

public class NavigationItemTests
{
    [Fact]
    public void Type_configuration_and_parent_identity_are_checked()
    {
        Assert.Throws<DomainException>(() => new NavigationItem(1, "Group", NavigationItemType.Group, 0, documentId: 2));
        Assert.Throws<DomainException>(() => new NavigationItem(1, "Document", NavigationItemType.Document, 0));
        Assert.Throws<DomainException>(() => new NavigationItem(1, "Link", NavigationItemType.ExternalLink, 0, externalUrl: " "));
        Assert.Throws<DomainException>(() => new NavigationItem(1, "Self", NavigationItemType.Group, 0, parentId: 1));
        Assert.Throws<DomainException>(() => new NavigationItem(1, "Order", NavigationItemType.Group, -1));
        Assert.Throws<DomainException>(() => new NavigationItem(1, " ", NavigationItemType.Group, 0));

        var item = new NavigationItem(1, "Docs", NavigationItemType.Document, 0, documentId: 2);
        Assert.True(item.IsVisible);
        Assert.Throws<DomainException>(() => item.ChangeParent(1));
        Assert.Throws<DomainException>(() => item.ChangeParent(-2));
        Assert.Throws<DomainException>(() => item.UpdateExternalUrl("https://example.com"));
    }

    [Fact]
    public void Display_parent_order_visibility_and_link_can_change()
    {
        var item = new NavigationItem(1, "External", NavigationItemType.ExternalLink, 0, externalUrl: "https://example.com");
        item.UpdateTitle("Reference");
        item.UpdateDisplay("reference", "link");
        item.ChangeParent(2);
        item.ChangeOrder(3);
        item.SetVisibility(false);
        item.UpdateExternalUrl("https://docs.example.com");
        Assert.Equal("Reference", item.Title);
        Assert.Equal("reference", item.Slug);
        Assert.Equal("link", item.Icon);
        Assert.Equal(2, item.ParentId);
        Assert.Equal(3, item.Order);
        Assert.False(item.IsVisible);
        Assert.Equal("https://docs.example.com", item.ExternalUrl);
        Assert.Throws<DomainException>(() => item.Id = 9);
        Assert.Throws<DomainException>(() => item.UpdateExternalUrl(" "));
        Assert.Throws<DomainException>(() => item.ChangeOrder(-1));
    }
}
