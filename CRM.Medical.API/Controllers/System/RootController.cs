using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.System;

[Route("")]
public sealed class RootController : SystemBaseController
{
    [HttpGet("")]
    [AllowAnonymous]
    [SwaggerOperation(
        OperationId = "Root_RedirectToSwagger",
        Summary = "Redirect to Swagger UI",
        Description = "Redirects root requests to the interactive API documentation page.")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult RedirectToSwagger()
    {
        return Redirect("/swagger");
    }
}
