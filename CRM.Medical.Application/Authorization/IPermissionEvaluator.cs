namespace CRM.Medical.Application.Authorization;

public interface IPermissionEvaluator
{
    Task<AuthorizationDecision> EvaluateAsync(
        PolicyEvaluationContext context,
        CancellationToken cancellationToken);
}
