using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.LockUser;

public sealed record LockUserCommand(string UserId, int LockoutMinutes) : IRequest;
