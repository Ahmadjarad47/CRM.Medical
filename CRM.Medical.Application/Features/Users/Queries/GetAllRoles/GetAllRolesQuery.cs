using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<IReadOnlyList<string>>;
