namespace DocumentViewer.Application.Users.Admin.Commands.Update;

public sealed record UpdateAdminUserResponse(int Id, string UserName, string FirstName, string LastName,
    string? Email, string Role, bool IsActive, DateTimeOffset CreatedOnUtc, DateTimeOffset? LastLoggedOnUtc);
