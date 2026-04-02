using CRM.Medical.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace CRM.Medical.API.ExceptionHandlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ApplicationExceptionBase appEx)
        {
            httpContext.Response.StatusCode = appEx.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = appEx.StatusCode,
                    Title = GetTitle(appEx.StatusCode),
                    Detail = appEx.Message,
                    Type = $"https://httpstatus.es/{appEx.StatusCode}",
                    Instance = httpContext.Request.Path
                },
                cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Type = "https://httpstatus.es/500",
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        _ => "An error occurred"
    };
}
