namespace DocumentViewer.Application.Users.Admin.Commands.Update;

public sealed record UpdateAdminUserRequest(string UserName, string FirstName, string LastName,
    string? Email, string Role);
