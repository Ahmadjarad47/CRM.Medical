using CRM.Medical.Application.Auth;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
