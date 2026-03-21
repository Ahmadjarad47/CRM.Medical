using System.Net;
using CRM.Medical.API.Health;
using CRM.Medical.Application.Features.Health.GetStatus;
using MediatR;

namespace CRM.Medical.API.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/status", StatusAsync).WithTags("Health");
        return app;
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
