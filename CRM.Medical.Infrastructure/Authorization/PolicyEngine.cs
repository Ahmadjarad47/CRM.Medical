using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Authorization;

public sealed class PolicyEngine(
    IPolicyProvider policyProvider,
    IConditionParser conditionParser,
    ILogger<PolicyEngine> logger) : IPolicyEngine
{
    public async Task<AuthorizationDecision> AuthorizeAsync(
        PolicyEvaluationContext context,
        CancellationToken cancellationToken)
    {
        var policies = await policyProvider.GetPoliciesAsync(context, cancellationToken);
        if (policies.Count == 0)
        {
            var denied = AuthorizationDecision.Deny("No matching policy (default deny).");
            Log(context, denied);
            return denied;
        }

        var matchedAllowReason = string.Empty;
        foreach (var policy in policies)
        {
            var predicate = conditionParser.ParseOrCompile(policy.Condition);
            if (!predicate(context))
                continue;

            if (policy.Effect == PolicyEffect.Deny)
            {
                var denyDecision = AuthorizationDecision.Deny($"Matched deny policy '{policy.Id}'.");
                Log(context, denyDecision);
                return denyDecision;
            }

            if (policy.Effect == PolicyEffect.Allow && matchedAllowReason.Length == 0)
                matchedAllowReason = $"Matched allow policy '{policy.Id}'.";
        }

        if (matchedAllowReason.Length > 0)
        {
            var allowDecision = AuthorizationDecision.Allow(matchedAllowReason);
            Log(context, allowDecision);
            return allowDecision;
        }

        var decision = AuthorizationDecision.Deny("Policies found but no condition matched.");
        Log(context, decision);
        return decision;
    }

    private void Log(PolicyEvaluationContext context, AuthorizationDecision decision)
    {
        logger.LogInformation(
            "Authorization decision: {Decision} user={UserId} permission={Permission} reason={Reason}",
            decision.IsAllowed ? "ALLOW" : "DENY",
            context.UserId,
            context.Permission.Key,
            decision.Reason);
    }
}
