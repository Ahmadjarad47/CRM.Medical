using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Doctor;

/// <summary>
/// Base for doctor-facing endpoints.
/// </summary>
[ApiController]
[Route("api/doctor")]
public abstract class DoctorBaseController : ControllerBase;
