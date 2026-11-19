using System.Text.Json;
using DocumentViewer.Domain.Documents;
using DocumentViewer.Domain.Documents.Content;
using DocumentViewer.Domain.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentViewer.Infrastructure.Sql.ContentManagement;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property<byte[]>("RowVersion").IsRowVersion().IsRequired();
        builder.HasMany(x => x.Components).WithOne().HasForeignKey("DocumentId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Components).HasField("_components")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class DocumentComponentConfiguration : IEntityTypeConfiguration<DocumentComponent>
{
    public void Configure(EntityTypeBuilder<DocumentComponent> builder)
    {
        builder.ToTable("DocumentComponents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Content).HasConversion(
            content => ContentJson.Serialize(content), json => ContentJson.Deserialize(json))
            .HasColumnType("nvarchar(max)");
        builder.Property<long>("DocumentId").IsRequired();
        builder.HasIndex("DocumentId", nameof(DocumentComponent.Order));
    }
}

public sealed class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PublishedAtUtc)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
        builder.HasOne<Document>().WithMany().HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.DocumentId, x.VersionNumber }).IsUnique();
    }
}

public sealed class NavigationItemConfiguration : IEntityTypeConfiguration<NavigationItem>
{
    public void Configure(EntityTypeBuilder<NavigationItem> builder)
    {
        builder.ToTable("NavigationItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(200);
        builder.Property(x => x.Icon).HasMaxLength(100);
        builder.Property(x => x.ExternalUrl).HasMaxLength(2048);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.HasOne<NavigationItem>().WithMany().HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Document>().WithMany().HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ParentId, x.Order });
        builder.Ignore(x => x.DomainEvents);
    }
}

internal static class ContentJson
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    internal static string Serialize(ComponentContent content) =>
        JsonSerializer.Serialize(new ContentEnvelope(content.GetType().Name, JsonSerializer.SerializeToElement(content, content.GetType(), Options)), Options);

    internal static ComponentContent Deserialize(string json)
    {
        var envelope = JsonSerializer.Deserialize<ContentEnvelope>(json, Options)
            ?? throw new JsonException("Missing component content.");
        var type = envelope.Type switch
        {
            nameof(HeadingContent) => typeof(HeadingContent),
            nameof(TextContent) => typeof(TextContent),
            nameof(CodeContent) => typeof(CodeContent),
            nameof(ImageContent) => typeof(ImageContent),
            nameof(TableContent) => typeof(TableContent),
            nameof(AlertContent) => typeof(AlertContent),
            nameof(QuoteContent) => typeof(QuoteContent),
            nameof(ListContent) => typeof(ListContent),
            nameof(VideoContent) => typeof(VideoContent),
            nameof(LinkContent) => typeof(LinkContent),
            nameof(DividerContent) => typeof(DividerContent),
            nameof(FileContent) => typeof(FileContent),
            nameof(EmbedContent) => typeof(EmbedContent),
            _ => throw new JsonException("Unknown component content type.")
        };
        return (ComponentContent?)envelope.Data.Deserialize(type, Options)
            ?? throw new JsonException("Missing component content.");
    }

    private sealed record ContentEnvelope(string Type, JsonElement Data);
}
