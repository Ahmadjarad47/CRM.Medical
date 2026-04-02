using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.ChangeMyPassword;

public sealed record ChangeMyPasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
