using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.LabPartner;

/// <summary>
/// Base for lab partner-facing endpoints.
/// </summary>
[ApiController]
[Route("api/labpartner")]
public abstract class LabPartnerBaseController : ControllerBase;
