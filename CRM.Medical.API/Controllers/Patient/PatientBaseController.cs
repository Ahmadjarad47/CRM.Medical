using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Patient;

/// <summary>
/// Base for patient-facing endpoints.
/// </summary>
[ApiController]
[Route("api/patient")]
public abstract class PatientBaseController : ControllerBase;
