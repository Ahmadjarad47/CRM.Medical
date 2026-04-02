using MediatR;

namespace CRM.Medical.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string Token) : IRequest;
