using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.LabPartner;

/// <summary>
/// Base for lab partner-facing endpoints. Requires the <see cref="UserRoles.LabPartner"/> role claim.
/// </summary>
[Authorize(Roles = UserRoles.LabPartner)]
[ApiController]
[Route("api/labpartner")]
public abstract class LabPartnerBaseController : ControllerBase;
