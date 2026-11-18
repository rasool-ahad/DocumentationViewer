using DocumentViewer.Domain.Documents.Enums;
using Framework.DDD.Domain.Guards;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Documents.Content;

public abstract record ComponentContent;

internal static class ContentRules
{
    internal static string Required(string? value, string name)
    {
        DomainGuard.ThrowIf.NotEmpty(value, $"{name} is required.");
        return value!;
    }

    internal static IReadOnlyCollection<T> Copy<T>(IEnumerable<T>? values, string name)
    {
        DomainGuard.ThrowIf.NotEqual(values is null, true, $"{name} is required.");
        var copy = values!.ToArray();
        DomainGuard.ThrowIf.NotEqual(copy.Any(x => x is null), true,
            $"{name} cannot contain null values.");
        return Array.AsReadOnly(copy);
    }
}

public sealed record HeadingContent : ComponentContent
{
    public string Text { get; }
    public int Level { get; }
    public HeadingContent(string text, int level)
    {
        Text = ContentRules.Required(text, nameof(Text));
        DomainGuard.ThrowIf.GreaterThanOrEqual(level, 1, "Heading level must be between 1 and 6.");
        DomainGuard.ThrowIf.LessThanOrEqual(level, 6, "Heading level must be between 1 and 6.");
        Level = level;
    }
}

public sealed record TextContent : ComponentContent
{
    public string Text { get; }
    public TextFormat Format { get; }
    public TextContent(string text, TextFormat format)
    {
        Text = ContentRules.Required(text, nameof(Text));
        DomainGuard.ThrowIf.NotEqual(Enum.IsDefined(format), false, "Invalid text format.");
        Format = format;
    }
}

public sealed record CodeContent : ComponentContent
{
    public string Code { get; }
    public string Language { get; }
    public bool ShowLineNumbers { get; }
    public IReadOnlyCollection<int> HighlightedLines { get; }
    public CodeContent(string code, string language, bool showLineNumbers, IReadOnlyCollection<int> highlightedLines)
    {
        Code = ContentRules.Required(code, nameof(Code));
        Language = ContentRules.Required(language, nameof(Language));
        ShowLineNumbers = showLineNumbers;
        var lines = ContentRules.Copy(highlightedLines, nameof(HighlightedLines));
        var lineCount = code.Split('\n').Length;
        DomainGuard.ThrowIf.NotEqual(lines.Any(x => x < 1 || x > lineCount), true,
            "Highlighted line is outside the code range.");
        HighlightedLines = lines;
    }
}

public sealed record ImageContent : ComponentContent
{
    public string Url { get; }
    public string Alt { get; }
    public string? Caption { get; }
    public ImageContent(string url, string alt, string? caption)
    {
        Url = ContentRules.Required(url, nameof(Url));
        Alt = ContentRules.Required(alt, nameof(Alt));
        Caption = caption;
    }
}

public sealed record TableRowContent
{
    public IReadOnlyCollection<string> Cells { get; }
    public TableRowContent(IReadOnlyCollection<string> cells) => Cells = ContentRules.Copy(cells, nameof(Cells));
}

public sealed record TableContent : ComponentContent
{
    public IReadOnlyCollection<string> Headers { get; }
    public IReadOnlyCollection<TableRowContent> Rows { get; }
    public TableContent(IReadOnlyCollection<string> headers, IReadOnlyCollection<TableRowContent> rows)
    {
        Headers = ContentRules.Copy(headers, nameof(Headers));
        Rows = ContentRules.Copy(rows, nameof(Rows));
        DomainGuard.ThrowIf.NotEqual(Headers.Count > 0 && Headers.All(x => !string.IsNullOrWhiteSpace(x)),
            false, "A table requires at least one nonempty header.");
        DomainGuard.ThrowIf.NotEqual(Rows.Any(row => row.Cells.Count != Headers.Count), true,
            "Every table row must match the header count.");
    }
}

public enum AlertVariant { Info = 1, Success = 2, Warning = 3, Danger = 4 }

public sealed record AlertContent : ComponentContent
{
    public string? Title { get; }
    public string Message { get; }
    public AlertVariant Variant { get; }
    public AlertContent(string? title, string message, AlertVariant variant)
    {
        Title = title;
        Message = ContentRules.Required(message, nameof(Message));
        DomainGuard.ThrowIf.NotEqual(Enum.IsDefined(variant), false, "Invalid alert variant.");
        Variant = variant;
    }
}

public sealed record QuoteContent : ComponentContent
{
    public string Text { get; }
    public string? Author { get; }
    public QuoteContent(string text, string? author)
    {
        Text = ContentRules.Required(text, nameof(Text));
        Author = author;
    }
}

public sealed record ListContent : ComponentContent
{
    public bool IsOrdered { get; }
    public IReadOnlyCollection<string> Items { get; }
    public ListContent(bool isOrdered, IReadOnlyCollection<string> items)
    {
        IsOrdered = isOrdered;
        Items = ContentRules.Copy(items, nameof(Items));
        DomainGuard.ThrowIf.NotEqual(Items.Count > 0 && Items.All(x => !string.IsNullOrWhiteSpace(x)),
            false, "A list requires at least one nonempty item.");
    }
}

public sealed record VideoContent : ComponentContent
{
    public string Url { get; }
    public string Provider { get; }
    public VideoContent(string url, string provider)
    {
        Url = ContentRules.Required(url, nameof(Url));
        Provider = ContentRules.Required(provider, nameof(Provider));
    }
}

public sealed record LinkContent : ComponentContent
{
    public string Text { get; }
    public string Url { get; }
    public bool OpenInNewTab { get; }
    public LinkContent(string text, string url, bool openInNewTab)
    {
        Text = ContentRules.Required(text, nameof(Text));
        Url = ContentRules.Required(url, nameof(Url));
        OpenInNewTab = openInNewTab;
    }
}

public sealed record DividerContent(string? Style) : ComponentContent;

public sealed record FileContent : ComponentContent
{
    public string FileName { get; }
    public string Url { get; }
    public string ContentType { get; }
    public FileContent(string fileName, string url, string contentType)
    {
        FileName = ContentRules.Required(fileName, nameof(FileName));
        Url = ContentRules.Required(url, nameof(Url));
        ContentType = ContentRules.Required(contentType, nameof(ContentType));
    }
}

public sealed record EmbedContent : ComponentContent
{
    public string Url { get; }
    public EmbedContent(string url) => Url = ContentRules.Required(url, nameof(Url));
}
