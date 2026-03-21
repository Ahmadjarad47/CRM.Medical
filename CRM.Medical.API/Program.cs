using System.Diagnostics;
using CRM.Medical.API.ExceptionHandlers;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application;
using CRM.Medical.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Instance ??= ctx.HttpContext.Request.Path.Value;
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        if (Activity.Current?.Id is { } activityId)
            ctx.ProblemDetails.Extensions["traceParent"] = activityId;

        if (builder.Environment.IsDevelopment() && ctx.Exception is not null)
            ctx.ProblemDetails.Extensions["exceptionType"] = ctx.Exception.GetType().FullName!;
    };
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddCrmSwagger();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    var statusCode = httpContext.Response.StatusCode;
    if (statusCode is < 400 or >= 600)
        return;

    var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
    var title = ReasonPhrases.GetReasonPhrase(statusCode);
    var problemDetails = new ProblemDetails
    {
        Status = statusCode,
        Title = string.IsNullOrEmpty(title) ? "An error occurred." : title,
        Type = $"https://httpstatus.es/{statusCode}",
        Instance = httpContext.Request.Path.Value,
    };

    await problemDetailsService.WriteAsync(new ProblemDetailsContext
    {
        HttpContext = httpContext,
        ProblemDetails = problemDetails,
    });
});

app.Logger.LogInformation(
    "Environment: {EnvironmentName}. URLs follow ASPNETCORE_URLS / launch profile (not hardcoded here).",
    app.Environment.EnvironmentName
);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Medical API v1");
    options.RoutePrefix = "swagger";
});

app.MapCrmEndpoints();

app.Run();
