namespace DocumentViewer.Application.Users.Admin.Commands.Create;

public sealed record CreateAdminUserRequest(string UserName, string FirstName, string LastName,
    string? Email, string Password, string Role);
