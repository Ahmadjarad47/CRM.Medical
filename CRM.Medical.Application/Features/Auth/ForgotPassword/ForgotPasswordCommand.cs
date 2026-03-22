using MediatR;

namespace CRM.Medical.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email, string ResetBaseUrl) : IRequest;
