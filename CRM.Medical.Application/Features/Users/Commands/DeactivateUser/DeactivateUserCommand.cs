using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(string UserId) : IRequest;
