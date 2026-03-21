using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.UnlockUser;

public sealed record UnlockUserCommand(string UserId) : IRequest;
