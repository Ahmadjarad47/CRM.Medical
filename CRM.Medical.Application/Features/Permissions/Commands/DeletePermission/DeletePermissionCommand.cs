using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.DeletePermission;

public sealed record DeletePermissionCommand(Guid Id) : IRequest;
