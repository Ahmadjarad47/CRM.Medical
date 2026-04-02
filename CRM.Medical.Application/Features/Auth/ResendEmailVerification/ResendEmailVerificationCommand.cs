using MediatR;

namespace CRM.Medical.Application.Features.Auth.ResendEmailVerification;

public sealed record ResendEmailVerificationCommand(string Email) : IRequest;
