using MediatR;

namespace CRM.Medical.Application.Features.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(string Id) : IRequest;
