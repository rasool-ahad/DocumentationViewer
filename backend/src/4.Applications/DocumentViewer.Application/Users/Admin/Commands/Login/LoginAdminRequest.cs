using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Application.Users.Admin.Commands.Login;

public sealed class LoginAdminRequest
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
