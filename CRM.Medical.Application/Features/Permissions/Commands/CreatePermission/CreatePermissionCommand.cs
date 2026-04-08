using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.CreatePermission;

public sealed record CreatePermissionCommand(string Name, string? Description) : IRequest<PermissionDto>;
