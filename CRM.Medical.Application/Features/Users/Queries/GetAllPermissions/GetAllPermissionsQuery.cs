using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery : IRequest<IReadOnlyList<string>>;
