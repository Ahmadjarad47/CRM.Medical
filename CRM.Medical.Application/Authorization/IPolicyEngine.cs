namespace CRM.Medical.Application.Authorization;

public interface IPolicyEngine
{
    Task<AuthorizationDecision> AuthorizeAsync(
        PolicyEvaluationContext context,
        CancellationToken cancellationToken);
}
