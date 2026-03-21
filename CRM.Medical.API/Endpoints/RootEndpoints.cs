namespace CRM.Medical.API.Endpoints;

public static class RootEndpoints
{
    public static IEndpointRouteBuilder MapRootEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Redirect("/swagger"))
            .WithName("Root_RedirectToSwagger")
            .WithSummary("Redirect to Swagger UI")
            .WithDescription("Redirects root requests to the interactive API documentation page.")
            .WithTags("Infrastructure")
            .Produces(StatusCodes.Status302Found)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
        return app;
    }
}
