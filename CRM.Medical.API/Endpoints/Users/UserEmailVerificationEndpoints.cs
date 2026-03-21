using CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;
using CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;
using MediatR;

namespace CRM.Medical.API.Endpoints.Users;

public static class UserEmailVerificationEndpoints
{
    private const string AdminRole = "Admin";

    public static RouteGroupBuilder MapUserEmailVerificationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{userId:guid}/email-verification", SendEmailVerificationAsync)
            .WithName("Users_EmailVerification_Send")
            .WithSummary("Send email verification to a user (Admin)")
            .WithDescription("Sends an email verification message to the specified user. Requires Admin role.")
            .WithTags("User Email Verification")
            .Accepts<SendEmailVerificationRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(AdminRole));

        group.MapPost("/{userId:guid}/confirm-email", ConfirmEmailAsync)
            .WithName("Users_EmailVerification_Confirm")
            .WithSummary("Confirm user email")
            .WithDescription("Confirms a user's email address using a one-time verification token.")
            .WithTags("User Email Verification")
            .Accepts<ConfirmEmailRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> SendEmailVerificationAsync(
        string userId,
        SendEmailVerificationRequest body,
        ISender mediator)
    {
        await mediator.Send(new SendEmailVerificationCommand(userId, body.ConfirmationBaseUrl));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> ConfirmEmailAsync(
        string userId,
        ConfirmEmailRequest body,
        ISender mediator)
    {
        await mediator.Send(new ConfirmEmailCommand(userId, body.Token));
        return TypedResults.NoContent();
    }

    public sealed record SendEmailVerificationRequest(string ConfirmationBaseUrl);

    public sealed record ConfirmEmailRequest(string Token);
}
