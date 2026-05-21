namespace CRM.Medical.Application.Authorization;

public interface IAccessPolicyReadService
{
    Task<IReadOnlyDictionary<string, IReadOnlyList<AccessPolicySummaryDto>>> GetPoliciesForRolesAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken);
}
