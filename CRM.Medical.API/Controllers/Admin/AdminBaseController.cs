using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

/// <summary>
/// Base for all admin-area controllers.
/// Shared base for admin-area controllers.
/// </summary>
[ApiController]
[Route("api/admin")]
public abstract class AdminBaseController : ControllerBase;
