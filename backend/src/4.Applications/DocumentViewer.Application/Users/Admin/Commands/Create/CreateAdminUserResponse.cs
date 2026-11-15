namespace DocumentViewer.Application.Users.Admin.Commands.Create;

public sealed record CreateAdminUserResponse(int Id, string UserName, string FirstName, string LastName,
    string? Email, string Role, bool IsActive, DateTimeOffset CreatedOnUtc, DateTimeOffset? LastLoggedOnUtc);
