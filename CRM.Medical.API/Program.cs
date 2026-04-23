using System.Diagnostics;
using System.Text.Json.Serialization;
using CRM.Medical.API.ExceptionHandlers;
using CRM.Medical.API.Extensions;
using CRM.Medical.API.Filters;
using CRM.Medical.API.Middlewares;
using CRM.Medical.API.Models;
using CRM.Medical.API.Services;
using CRM.Medical.Application;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Infrastructure;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 12 * 1024 * 1024;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 12 * 1024 * 1024;
});

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

builder.Services.AddApplication();
builder.Services.AddCrmSwagger();
builder.Services.AddCrmMiddlewares(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = new List<string>();
        foreach (var (key, entry) in context.ModelState)
        {
            if (entry.Errors is not { Count: > 0 })
                continue;

            var prefix = string.IsNullOrEmpty(key) ? "Field" : key;
            foreach (var err in entry.Errors)
            {
                if (string.IsNullOrEmpty(err.ErrorMessage))
                    errors.Add(prefix + " is invalid.");
                else
                    errors.Add(
                        err.ErrorMessage.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                            ? err.ErrorMessage
                            : $"{prefix}: {err.ErrorMessage}");
            }
        }

        if (errors.Count == 0)
            errors.Add("The request was invalid or could not be read.");

        return new ObjectResult(ApiEnvelope.ValidationFailed(errors))
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    };
});

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ApiEnvelopeResultFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Apply migrations before IHostedService seeding (e.g. permission catalog) touches the database.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
        await db.Database.MigrateAsync();
}

app.UseCrmMiddlewares();
app.UseCrmErrorHandling();

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    var statusCode = httpContext.Response.StatusCode;
    if (statusCode is < 400 or >= 600)
        return;

    var title = ReasonPhrases.GetReasonPhrase(statusCode);
    await httpContext.Response.WriteAsJsonAsync(
        ApiEnvelope.FromHttpStatusCode(statusCode, string.IsNullOrEmpty(title) ? null : title),
        httpContext.RequestAborted);
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

app.MapControllers();

app.Run();
