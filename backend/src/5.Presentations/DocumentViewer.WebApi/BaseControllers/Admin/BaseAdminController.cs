using DocumentViewer.Contract.Users.Admin;
using Framework.DDD.Presentation.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentViewer.WebApi.BaseControllers.Admin;

[Route("api/admin/[controller]")]
[Authorize(Roles = AdminRoles.Admin + "," + AdminRoles.SuperAdmin)]
[ApiExplorerSettings(GroupName = "DocumentViewer.Admin")]
public abstract class BaseAdminController : BaseController;
