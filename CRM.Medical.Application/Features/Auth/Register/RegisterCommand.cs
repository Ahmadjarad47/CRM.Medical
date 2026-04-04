using MediatR;

namespace CRM.Medical.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    string Role) : IRequest<RegisterResponse>;
