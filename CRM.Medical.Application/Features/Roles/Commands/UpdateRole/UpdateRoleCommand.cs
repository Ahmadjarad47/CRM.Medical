using MediatR;

namespace CRM.Medical.Application.Features.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(string Id, string Name) : IRequest;
