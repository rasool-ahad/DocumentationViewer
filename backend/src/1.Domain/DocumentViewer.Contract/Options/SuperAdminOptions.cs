namespace DocumentViewer.Contract.Options;

public sealed class SuperAdminOptions
{
    public const string SectionName = "SuperAdmin";

    public string UserName { get; set; } = "superadmin";
    public string FirstName { get; set; } = "Super";
    public string LastName { get; set; } = "Admin";
    public string Password { get; set; } = string.Empty;
}
