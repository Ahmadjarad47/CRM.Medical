namespace CRM.Medical.API.Contracts.User.Auth;

public sealed record ConfirmAccountDeletionRequest(string Email, string Token);
