using CRM.Medical.API.ExceptionHandlers;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application;
using CRM.Medical.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddCrmSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

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
