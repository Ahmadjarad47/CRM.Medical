using CRM.Medical.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace CRM.Medical.API.ExceptionHandlers;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;

        var errorMessages = validationException.Errors
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.PropertyName : e.ErrorMessage)
            .Where(s => s.Length > 0)
            .Distinct()
            .ToList();

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            ApiEnvelope.ValidationFailed(errorMessages),
            cancellationToken);

        return true;
    }
}
