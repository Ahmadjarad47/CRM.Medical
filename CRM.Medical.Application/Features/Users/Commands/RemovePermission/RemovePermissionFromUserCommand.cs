using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.RemovePermission;

public sealed record RemovePermissionFromUserCommand(
    string UserId,
    string Permission) : IRequest;
