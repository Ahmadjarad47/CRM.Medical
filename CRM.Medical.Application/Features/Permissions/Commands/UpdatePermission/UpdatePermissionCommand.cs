using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.UpdatePermission;

public sealed record UpdatePermissionCommand(Guid Id, string? Description) : IRequest;
