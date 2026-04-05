using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Queries.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<IReadOnlyList<PermissionDto>>;
