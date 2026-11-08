using Framework.DDD.Presentation.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentViewer.WebApi.BaseControllers.My;

[Route("api/my/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "DocumentViewer.My")]
public abstract class BaseMyController : BaseController;
