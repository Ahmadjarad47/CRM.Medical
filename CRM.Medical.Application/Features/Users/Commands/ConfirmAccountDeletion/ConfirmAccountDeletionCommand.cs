using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.ConfirmAccountDeletion;

/// <summary>
/// Verifies the deletion token sent by email and permanently deletes the account.
/// Uses email + token so the user can confirm even without an active session.
/// </summary>
public sealed record ConfirmAccountDeletionCommand(string Email, string Token) : IRequest;
