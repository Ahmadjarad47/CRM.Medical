using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(string UserId) : IRequest;
