namespace CRM.Medical.Application.Authorization;

public interface IPolicyProvider
{
    Task<IReadOnlyList<AbacPolicyDefinition>> GetPoliciesAsync(
        PolicyEvaluationContext context,
        CancellationToken cancellationToken);
}
