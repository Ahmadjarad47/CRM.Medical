using System.Net;
using CRM.Medical.API.Health;
using CRM.Medical.Application.Features.Health.GetStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.System;

[Route("")]
public sealed class HealthController(ISender sender) : SystemBaseController
{
    [HttpGet("status")]
    [AllowAnonymous]
    [Produces("text/html")]
    [SwaggerOperation(
        OperationId = "Health_StatusPage",
        Summary = "Render health status page",
        Description = "Renders an HTML page with current environment and database connectivity status.")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> StatusAsync()
    {
        var model = await sender.Send(new GetHealthStatusQuery());
        var envName = WebUtility.HtmlEncode(model.EnvironmentName);
        var safeStatusText = WebUtility.HtmlEncode(model.StatusPlainText);

        var html = StatusPageTemplate.Html
            .Replace("__ENV__", envName, StringComparison.Ordinal)
            .Replace("__STATUS_CLASS__", model.StatusCssClass, StringComparison.Ordinal)
            .Replace("__STATUS_TEXT__", safeStatusText, StringComparison.Ordinal);

        return Content(html, "text/html; charset=utf-8");
    }
}
