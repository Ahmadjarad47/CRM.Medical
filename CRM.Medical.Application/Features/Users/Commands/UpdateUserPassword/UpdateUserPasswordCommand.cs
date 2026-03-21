using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;

public sealed record UpdateUserPasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
