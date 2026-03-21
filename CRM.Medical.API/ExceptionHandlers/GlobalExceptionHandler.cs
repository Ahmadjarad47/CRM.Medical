using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Medical.API.ExceptionHandlers;

/// <summary>
/// Fallback handler for all unhandled exceptions. Runs after more specific <see cref="IExceptionHandler"/> implementations.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var mapped = MapException(exception);

        logger.Log(
            mapped.LogLevel,
            exception,
            "HTTP {Method} {Path} failed with {StatusCode} ({ExceptionType})",
            httpContext.Request.Method,
            httpContext.Request.Path,
            mapped.Status,
            exception.GetType().Name);

        var problemDetails = new ProblemDetails
        {
            Status = mapped.Status,
            Title = mapped.Title,
            Type = mapped.Type,
            Detail = environment.IsDevelopment() ? exception.ToString() : null,
            Instance = httpContext.Request.Path.Value,
        };

        if (Activity.Current?.Id is { } activityId)
            problemDetails.Extensions["traceParent"] = activityId;

        httpContext.Response.StatusCode = mapped.Status;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        });

        return true;
    }

    private static MappedException MapException(Exception exception) =>
        exception switch
        {
            BadHttpRequestException badReq => new(
                badReq.StatusCode,
                string.IsNullOrWhiteSpace(badReq.Message)
                    ? ReasonPhrases.GetReasonPhrase(badReq.StatusCode)
                    : badReq.Message,
                ProblemTypes.BadRequest,
                LogLevel.Warning),

            ArgumentNullException => new(
                StatusCodes.Status400BadRequest,
                "A required argument was missing.",
                ProblemTypes.BadRequest,
                LogLevel.Warning),

            ArgumentException argEx => new(
                StatusCodes.Status400BadRequest,
                string.IsNullOrWhiteSpace(argEx.Message) ? "The request was invalid." : argEx.Message,
                ProblemTypes.BadRequest,
                LogLevel.Warning),

            UnauthorizedAccessException => new(
                StatusCodes.Status403Forbidden,
                "You are not allowed to perform this action.",
                ProblemTypes.Forbidden,
                LogLevel.Warning),

            KeyNotFoundException => new(
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                ProblemTypes.NotFound,
                LogLevel.Information),

            FileNotFoundException => new(
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                ProblemTypes.NotFound,
                LogLevel.Information),

            NotImplementedException => new(
                StatusCodes.Status501NotImplemented,
                "This feature is not implemented.",
                ProblemTypes.NotImplemented,
                LogLevel.Warning),

            OperationCanceledException => new(
                StatusCodes.Status499ClientClosedRequest,
                "The request was cancelled.",
                ProblemTypes.ClientClosedRequest,
                LogLevel.Information),

            TimeoutException => new(
                StatusCodes.Status504GatewayTimeout,
                "The operation timed out.",
                ProblemTypes.GatewayTimeout,
                LogLevel.Warning),

            _ when IsEfConcurrencyException(exception) => new(
                StatusCodes.Status409Conflict,
                "The record was modified by another request. Refresh and try again.",
                ProblemTypes.Conflict,
                LogLevel.Warning),

            _ => new(
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                ProblemTypes.InternalServerError,
                LogLevel.Error),
        };

    /// <summary>
    /// Avoids a direct EF Core package reference from the API project.
    /// </summary>
    private static bool IsEfConcurrencyException(Exception exception) =>
        string.Equals(
            exception.GetType().FullName,
            "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException",
            StringComparison.Ordinal);

    private readonly record struct MappedException(
        int Status,
        string Title,
        string Type,
        LogLevel LogLevel);

    private static class ProblemTypes
    {
        public const string BadRequest = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
        public const string Forbidden = "https://tools.ietf.org/html/rfc9110#section-15.5.4";
        public const string NotFound = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
        public const string Conflict = "https://tools.ietf.org/html/rfc9110#section-15.5.10";
        public const string InternalServerError = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
        public const string NotImplemented = "https://tools.ietf.org/html/rfc9110#section-15.6.2";
        public const string GatewayTimeout = "https://tools.ietf.org/html/rfc9110#section-15.6.5";
        public const string ClientClosedRequest = "about:blank";
    }
}
