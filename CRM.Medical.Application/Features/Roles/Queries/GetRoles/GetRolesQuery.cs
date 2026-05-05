using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Roles.Queries.GetRoles;

public sealed record GetRolesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<PagedResult<RoleDto>>;
