using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(string UserId) : IRequest<UserRolesDto>;
