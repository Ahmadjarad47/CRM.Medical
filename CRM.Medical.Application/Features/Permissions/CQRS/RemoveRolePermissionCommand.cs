using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record RemoveRolePermissionCommand(string RoleId, Guid PolicyId) : IRequest<Unit>;
