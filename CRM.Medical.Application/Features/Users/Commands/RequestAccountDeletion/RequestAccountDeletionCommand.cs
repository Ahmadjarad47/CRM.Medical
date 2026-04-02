using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.RequestAccountDeletion;

/// <summary>
/// Sends an account-deletion confirmation email to the authenticated user.
/// The email contains a time-limited token the user must submit to actually delete the account.
/// </summary>
public sealed record RequestAccountDeletionCommand(string UserId) : IRequest;
