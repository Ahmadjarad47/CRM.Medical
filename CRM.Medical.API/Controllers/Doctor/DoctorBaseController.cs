using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Doctor;

/// <summary>
/// Base for doctor-facing endpoints. Requires the <see cref="UserRoles.Doctor"/> role claim.
/// </summary>
[Authorize(Roles = UserRoles.Doctor)]
[ApiController]
[Route("api/doctor")]
public abstract class DoctorBaseController : ControllerBase;
