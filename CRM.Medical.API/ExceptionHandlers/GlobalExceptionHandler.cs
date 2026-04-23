using CRM.Medical.API.Models;
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
                ApiEnvelope.FromApplicationException(appEx),
                cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            ApiEnvelope.InternalServerError(),
            cancellationToken);

        return true;
    }
}
