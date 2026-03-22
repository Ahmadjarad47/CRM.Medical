using CRM.Medical.Application.Auth;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string UserId, string RefreshToken) : IRequest<LoginResponse>;
