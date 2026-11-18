using DocumentViewer.Domain.Documents.Content;
using DocumentViewer.Domain.Documents.Enums;
using Framework.DDD.Domain.Exceptions;
using Xunit;

namespace DocumentViewer.Domain.UnitTests.Documents;

public class ContentTests
{
    [Fact]
    public void Every_component_type_accepts_its_content_model()
    {
        var samples = new (ComponentType Type, ComponentContent Content)[]
        {
            (ComponentType.Heading, new HeadingContent("Title", 1)),
            (ComponentType.Text, new TextContent("Text", TextFormat.PlainText)),
            (ComponentType.Code, new CodeContent("code", "cs", false, [])),
            (ComponentType.Image, new ImageContent("/image", "alt", null)),
            (ComponentType.Table, new TableContent(["H"], [new TableRowContent(["C"])])),
            (ComponentType.Alert, new AlertContent(null, "Alert", AlertVariant.Info)),
            (ComponentType.Quote, new QuoteContent("Quote", null)),
            (ComponentType.List, new ListContent(false, ["Item"])),
            (ComponentType.Video, new VideoContent("/video", "provider")),
            (ComponentType.Link, new LinkContent("Link", "/target", false)),
            (ComponentType.Divider, new DividerContent(null)),
            (ComponentType.File, new FileContent("file", "/file", "text/plain")),
            (ComponentType.Embed, new EmbedContent("/embed"))
        };

        var document = new DocumentViewer.Domain.Documents.Document(1, "Guide", "guide", null);
        foreach (var (type, content) in samples)
            document.AddComponent((long)type, type, content);
        Assert.Equal(13, document.Components.Count);
        Assert.Throws<DomainException>(() => document.AddComponent(100, (ComponentType)100, samples[0].Content));
    }

    [Fact]
    public void Heading_and_code_enforce_semantic_rules()
    {
        Assert.Throws<DomainException>(() => new HeadingContent(" ", 1));
        Assert.Throws<DomainException>(() => new HeadingContent("Title", 7));
        Assert.Throws<DomainException>(() => new CodeContent("a\nb", "cs", false, [3]));
        Assert.Throws<DomainException>(() => new CodeContent("a", "cs", false, [0]));
        Assert.Throws<DomainException>(() => new TextContent("text", (TextFormat)99));
    }

    [Fact]
    public void Collection_content_copies_input_and_validates_shape()
    {
        var lines = new List<int> { 1 };
        var code = new CodeContent("a\nb", "cs", true, lines);
        lines[0] = 2;
        Assert.Equal([1], code.HighlightedLines);

        var cells = new List<string> { "value" };
        var table = new TableContent(["Header"], [new TableRowContent(cells)]);
        cells[0] = "changed";
        Assert.Equal("value", table.Rows.Single().Cells.Single());
        Assert.Throws<DomainException>(() => new TableContent([], []));
        Assert.Throws<DomainException>(() => new TableContent(["A", "B"], [new TableRowContent(["one"])]));
        Assert.Throws<DomainException>(() => new ListContent(false, []));
        Assert.Throws<DomainException>(() => new AlertContent(null, "message", (AlertVariant)0));
    }
}
