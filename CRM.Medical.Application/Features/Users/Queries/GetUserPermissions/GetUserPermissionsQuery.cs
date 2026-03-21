using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string UserId) : IRequest<UserPermissionsDto>;
