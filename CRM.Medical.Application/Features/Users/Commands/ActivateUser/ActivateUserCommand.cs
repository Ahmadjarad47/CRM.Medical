using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(string UserId) : IRequest;
