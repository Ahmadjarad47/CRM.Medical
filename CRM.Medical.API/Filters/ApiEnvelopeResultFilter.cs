using System.Linq;
using CRM.Medical.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.Medical.API.Filters;

/// <summary>
/// Wraps successful action results in <see cref="ApiEnvelope"/> so clients always get <c>ok</c> / <c>bad</c> messaging.
/// </summary>
public sealed class ApiEnvelopeResultFilter : IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!ShouldWrap(context))
        {
            return next();
        }

        TransformResult(context);
        return next();
    }

    private static bool ShouldWrap(ResultExecutingContext context)
    {
        if (context.HttpContext.Request.Path.StartsWithSegments("/swagger"))
            return false;

        return true;
    }

    private static void TransformResult(ResultExecutingContext context)
    {
        switch (context.Result)
        {
            case NoContentResult:
                context.Result = new ObjectResult(ApiEnvelope.Ok())
                {
                    StatusCode = StatusCodes.Status200OK
                };
                return;

            case StatusCodeResult { StatusCode: StatusCodes.Status204NoContent }:
                context.Result = new ObjectResult(ApiEnvelope.Ok())
                {
                    StatusCode = StatusCodes.Status200OK
                };
                return;

            case OkResult:
                context.Result = new ObjectResult(ApiEnvelope.Ok())
                {
                    StatusCode = StatusCodes.Status200OK
                };
                return;

            case CreatedAtActionResult created:
                if (IsAlreadyEnvelope(created.Value))
                    return;
                context.Result = new CreatedAtActionResult(
                    created.ActionName,
                    created.ControllerName,
                    created.RouteValues,
                    ApiEnvelope.Ok(created.Value))
                {
                    StatusCode = created.StatusCode
                };
                return;

            case CreatedAtRouteResult createdRoute:
                if (IsAlreadyEnvelope(createdRoute.Value))
                    return;
                context.Result = new CreatedAtRouteResult(
                    createdRoute.RouteName,
                    createdRoute.RouteValues,
                    ApiEnvelope.Ok(createdRoute.Value))
                {
                    StatusCode = createdRoute.StatusCode
                };
                return;

            case CreatedResult created:
                if (IsAlreadyEnvelope(created.Value))
                    return;
                context.Result = new CreatedResult(created.Location, ApiEnvelope.Ok(created.Value))
                {
                    StatusCode = created.StatusCode
                };
                return;

            case ObjectResult obj:
                if (obj.Value is FileResult)
                    return;
                if (IsAlreadyEnvelope(obj.Value))
                    return;

                var statusCode = obj.StatusCode ?? context.HttpContext.Response.StatusCode;
                if (statusCode is >= 200 and < 300)
                {
                    obj.Value = ApiEnvelope.Ok(obj.Value);
                    obj.StatusCode = statusCode;
                    return;
                }

                if (statusCode is >= 400 and < 600)
                {
                    obj.Value = WrapError(obj.Value);
                    obj.DeclaredType = typeof(ApiEnvelope);
                }

                return;

            default:
                return;
        }
    }

    private static object WrapError(object? value)
    {
        if (value is ValidationProblemDetails vp)
        {
            var messages = vp.Errors.Values.SelectMany(m => m).ToList();
            if (messages.Count == 0 && !string.IsNullOrEmpty(vp.Detail))
                messages.Add(vp.Detail);
            if (messages.Count == 0 && !string.IsNullOrEmpty(vp.Title))
                messages.Add(vp.Title);
            return ApiEnvelope.ValidationFailed(messages);
        }

        if (value is ProblemDetails p)
        {
            return new ApiEnvelope
            {
                Success = false,
                Message = p.Title ?? "Error",
                Detail = p.Detail
            };
        }

        return new ApiEnvelope
        {
            Success = false,
            Message = "Error",
            Data = value
        };
    }

    private static bool IsAlreadyEnvelope(object? value) => value is ApiEnvelope;
}
