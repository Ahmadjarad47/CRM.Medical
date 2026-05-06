using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

/// <summary>
/// Base for all admin-area controllers.
/// Requires the caller to be authenticated; individual endpoints carry
/// the specific <c>[DynamicAuthorize(Resource, Action)]</c> (ABAC) attribute on each action.
/// </summary>
[Authorize]
[ApiController]
[Route("api/admin")]
public abstract class AdminBaseController : ControllerBase;
