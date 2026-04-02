using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

/// <summary>
/// Base for all admin-area controllers.
/// Requires the caller to be authenticated; individual endpoints carry
/// the specific [Authorize(Policy = "...")] attribute.
/// </summary>
[Authorize]
[ApiController]
[Route("api/admin")]
public abstract class AdminBaseController : ControllerBase;
