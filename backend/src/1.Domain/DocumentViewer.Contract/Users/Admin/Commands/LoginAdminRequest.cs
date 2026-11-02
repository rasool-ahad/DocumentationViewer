using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Contract.Users.Admin.Commands;

public sealed class LoginAdminRequest
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
