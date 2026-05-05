namespace CRM.Medical.Application.Authorization;

public interface IConditionParser
{
    Func<PolicyEvaluationContext, bool> ParseOrCompile(string? condition);
}
