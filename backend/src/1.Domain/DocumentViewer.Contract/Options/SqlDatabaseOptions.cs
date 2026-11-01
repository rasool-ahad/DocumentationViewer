namespace DocumentViewer.Contract.Options;

public sealed class SqlDatabaseOptions
{
    public const string SectionName = "SqlDatabase";

    public string ConnectionString { get; set; } = string.Empty;
}

