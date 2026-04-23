namespace CRM.Medical.API.Middlewares;

/// <summary>
/// Wires the ASP.NET Core global exception handler pipeline. Handlers
/// (validation + application + unhandled) are registered via
/// <c>AddExceptionHandler&lt;T&gt;()</c> and run when this middleware is active.
/// </summary>
public static class GlobalErrorHandlingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCrmErrorHandling(this IApplicationBuilder app) =>
        app.UseExceptionHandler();
}
