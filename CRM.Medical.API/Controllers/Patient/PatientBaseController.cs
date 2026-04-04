using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Patient;

/// <summary>
/// Base for patient-facing endpoints. Requires the <see cref="UserRoles.Patient"/> role claim.
/// </summary>
[Authorize(Roles = UserRoles.Patient)]
[ApiController]
[Route("api/patient")]
public abstract class PatientBaseController : ControllerBase;
