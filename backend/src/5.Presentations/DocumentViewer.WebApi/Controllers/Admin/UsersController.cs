using DocumentViewer.Application.Users.Admin.Commands.Create;
using DocumentViewer.Application.Users.Admin.Commands.Delete;
using DocumentViewer.Application.Users.Admin.Commands.Login;
using DocumentViewer.Application.Users.Admin.Commands.Logout;
using DocumentViewer.Application.Users.Admin.Commands.SetPassword;
using DocumentViewer.Application.Users.Admin.Commands.SetStatus;
using DocumentViewer.Application.Users.Admin.Commands.Update;
using DocumentViewer.Application.Users.Admin.Queries.Get;
using DocumentViewer.Application.Users.Admin.Queries.List;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.WebApi.BaseControllers.Admin;
using Framework.DDD.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentViewer.WebApi.Controllers.Admin;

public sealed class UsersController(ICommandQueryBus bus) : BaseAdminController
{
    [HttpGet]
    public Task<ListAdminUsersResponse> List(
        [FromQuery] ListAdminUsersRequest request, CancellationToken cancellationToken) =>
        bus.SendAsync(new ListAdminUsersQuery(request), cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<GetAdminUserResponse> Detail(int id, CancellationToken cancellationToken) =>
        bus.SendAsync(new GetAdminUserQuery(id), cancellationToken);

    [HttpPost]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<CreateAdminUserResponse> Create([FromBody] CreateAdminUserRequest request, CancellationToken cancellationToken) =>
        bus.SendAsync(new CreateAdminUserCommand(request, ActorId()), cancellationToken);

    [HttpPut("{id:int}")]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<UpdateAdminUserResponse> Update(int id, [FromBody] UpdateAdminUserRequest request,
        CancellationToken cancellationToken) =>
        bus.SendAsync(new UpdateAdminUserCommand(id, request, ActorId()), cancellationToken);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<DeleteAdminUserResponse> Delete(int id, CancellationToken cancellationToken) =>
        bus.SendAsync(new DeleteAdminUserCommand(id, ActorId()), cancellationToken);

    [HttpPut("{id:int}/password")]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<SetAdminPasswordResponse> SetPassword(int id, [FromBody] SetAdminPasswordRequest request,
        CancellationToken cancellationToken) =>
        bus.SendAsync(new SetAdminPasswordCommand(id, request, ActorId()), cancellationToken);

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = AdminRoles.SuperAdmin)]
    public Task<SetAdminStatusResponse> SetStatus(int id, [FromBody] SetAdminStatusRequest request,
        CancellationToken cancellationToken) =>
        bus.SendAsync(new SetAdminStatusCommand(id, request, ActorId()), cancellationToken);

    private int ActorId() => int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
        ? id : throw new Framework.DDD.Application.Exceptions.ApplicationUnauthorizedException("Invalid user identity.");

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<LoginAdminResponse> Login([FromBody] LoginAdminRequest request, CancellationToken cancellationToken)
    {
        return bus.SendAsync(new LoginAdminCommand(request.UserName, request.Password), cancellationToken);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(LogoutAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<LogoutAdminResponse> Logout(CancellationToken cancellationToken)
    {
        return bus.SendAsync(new LogoutAdminCommand(User.FindFirstValue(JwtRegisteredClaimNames.Jti),
            User.FindFirstValue(JwtRegisteredClaimNames.Exp)), cancellationToken);
    }
}
