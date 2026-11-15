namespace DocumentViewer.Application.Users.Admin.Queries.List;

public sealed record AdminUserItem(int Id, string UserName, string FirstName, string LastName,
    string? Email, string Role, bool IsActive, DateTimeOffset CreatedOnUtc, DateTimeOffset? LastLoggedOnUtc);
