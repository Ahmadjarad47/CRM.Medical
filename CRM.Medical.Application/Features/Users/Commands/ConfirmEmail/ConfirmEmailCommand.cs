using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string UserId, string Token) : IRequest;
