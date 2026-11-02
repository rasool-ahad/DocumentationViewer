using DocumentViewer.Application.Users.Admin.Commands;
using DocumentViewer.Contract.Users.Admin.Commands;
using DocumentViewer.WebApi.BaseControllers.Admin;
using Framework.DDD.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentViewer.WebApi.Controllers.Admin;

public sealed class UsersController(ICommandQueryBus bus) : BaseAdminController
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<LoginAdminResponse> Login([FromBody] LoginAdminRequest request, CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";
        return bus.SendAsync(new LoginAdminCommand(request.UserName, request.Password), cancellationToken);
    }
}
