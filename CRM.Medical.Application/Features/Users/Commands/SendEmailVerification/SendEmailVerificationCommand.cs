using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;

public sealed record SendEmailVerificationCommand(string UserId, string ConfirmationBaseUrl) : IRequest;
