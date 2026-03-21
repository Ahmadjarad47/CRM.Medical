using System.Net;
using CRM.Medical.API.Health;
using CRM.Medical.Application.Features.Health.GetStatus;
using MediatR;

namespace CRM.Medical.API.Endpoints.Health;

public static class StatusEndpoint
{
    public static RouteGroupBuilder MapStatusEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/status", StatusAsync)
            .WithName("Health_StatusPage")
            .WithSummary("Render health status page")
            .WithDescription("Renders an HTML page with current environment and database connectivity status.")
            .WithTags("Health")
            .Produces(StatusCodes.Status200OK, contentType: "text/html")
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> StatusAsync(ISender mediator)
    {
        var model = await mediator.Send(new GetHealthStatusQuery());
        var envName = WebUtility.HtmlEncode(model.EnvironmentName);
        var safeStatusText = WebUtility.HtmlEncode(model.StatusPlainText);

        var html = StatusPageTemplate.Html
            .Replace("__ENV__", envName, StringComparison.Ordinal)
            .Replace("__STATUS_CLASS__", model.StatusCssClass, StringComparison.Ordinal)
            .Replace("__STATUS_TEXT__", safeStatusText, StringComparison.Ordinal);

        return Results.Content(html, "text/html; charset=utf-8");
    }
}
