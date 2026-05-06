namespace CRM.Medical.Application.Authorization;

public static class AccessPolicyQueryableExtensions
{
    public static Task<IQueryable<TEntity>> ApplyAccessPolicyAsync<TEntity>(
        this IQueryable<TEntity> query,
        IAccessPolicyEvaluator evaluator,
        string resource,
        string action,
        CancellationToken cancellationToken) =>
        evaluator.ApplyAsync(query, resource, action, cancellationToken);
}
