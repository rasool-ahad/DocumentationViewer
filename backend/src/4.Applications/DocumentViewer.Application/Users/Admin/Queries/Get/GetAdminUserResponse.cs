namespace DocumentViewer.Application.Users.Admin.Queries.Get;

public sealed record GetAdminUserResponse(int Id, string UserName, string FirstName, string LastName,
    string? Email, string Role, bool IsActive, DateTimeOffset CreatedOnUtc, DateTimeOffset? LastLoggedOnUtc);
