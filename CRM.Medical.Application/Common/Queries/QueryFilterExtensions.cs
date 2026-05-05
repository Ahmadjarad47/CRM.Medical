using System.Linq.Expressions;
using System.Text.RegularExpressions;
using CRM.Medical.Application.Common.Responses;

namespace CRM.Medical.Application.Common.Queries;

public static partial class QueryFilterExtensions
{
    private sealed record SearchToken(string? Field, string Value, bool IsNegative);

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        return query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize);
    }

    public static IQueryable<T> ApplyAdvancedSearch<T>(
        this IQueryable<T> query,
        string? search,
        IReadOnlyDictionary<string, Expression<Func<T, string?>>> fieldSelectors,
        params Expression<Func<T, string?>>[] defaultSelectors)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var tokens = ParseSearch(search);
        if (tokens.Count == 0)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;

        foreach (var token in tokens)
        {
            var selectorSet = ResolveSelectors(token.Field, fieldSelectors, defaultSelectors);
            if (selectorSet.Count == 0)
                continue;

            var tokenValue = token.Value.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(tokenValue))
                continue;

            Expression? orPredicate = null;
            foreach (var selector in selectorSet)
            {
                var selectorBody = ReplaceParameter(selector.Body, selector.Parameters[0], parameter);
                var notNull = Expression.NotEqual(selectorBody, Expression.Constant(null, typeof(string)));
                var lowered = Expression.Call(selectorBody, typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                var contains = BuildWildcardContainsExpression(lowered, tokenValue);
                var fieldPredicate = Expression.AndAlso(notNull, contains);
                orPredicate = orPredicate is null ? fieldPredicate : Expression.OrElse(orPredicate, fieldPredicate);
            }

            if (orPredicate is null)
                continue;

            var termPredicate = token.IsNegative ? Expression.Not(orPredicate) : orPredicate;
            combined = combined is null ? termPredicate : Expression.AndAlso(combined, termPredicate);
        }

        if (combined is null)
            return query;

        var lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);
        return query.Where(lambda);
    }

    private static IReadOnlyList<Expression<Func<T, string?>>> ResolveSelectors<T>(
        string? field,
        IReadOnlyDictionary<string, Expression<Func<T, string?>>> fieldSelectors,
        Expression<Func<T, string?>>[] defaultSelectors)
    {
        if (field is null)
            return defaultSelectors;

        if (fieldSelectors.TryGetValue(field.ToLowerInvariant(), out var selector))
            return [selector];

        return [];
    }

    private static List<SearchToken> ParseSearch(string input)
    {
        var tokens = new List<SearchToken>();
        foreach (Match match in SearchRegex().Matches(input))
        {
            var raw = match.Groups[1].Success ? match.Groups[1].Value : match.Value;
            raw = raw.Trim();
            if (raw.Length == 0)
                continue;

            var isNegative = raw.StartsWith("-", StringComparison.Ordinal);
            if (isNegative)
                raw = raw[1..].Trim();

            if (raw.Length == 0)
                continue;

            var separator = raw.IndexOf(':');
            if (separator > 0 && separator < raw.Length - 1)
            {
                var field = raw[..separator].Trim();
                var value = raw[(separator + 1)..].Trim().Trim('"');
                if (field.Length > 0 && value.Length > 0)
                    tokens.Add(new SearchToken(field, value, isNegative));
                continue;
            }

            tokens.Add(new SearchToken(null, raw.Trim('"'), isNegative));
        }

        return tokens;
    }

    private static Expression BuildWildcardContainsExpression(Expression loweredField, string token)
    {
        var pieces = token.Split('*', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (pieces.Length == 0)
            return Expression.Constant(true);

        Expression? expression = null;
        foreach (var piece in pieces)
        {
            var contains = Expression.Call(
                loweredField,
                typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                Expression.Constant(piece));

            expression = expression is null ? contains : Expression.AndAlso(expression, contains);
        }

        return expression ?? Expression.Constant(true);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter) =>
        new ReplaceParameterVisitor(oldParameter, newParameter).Visit(expression)!;

    [GeneratedRegex("\"([^\"]+)\"|\\S+")]
    private static partial Regex SearchRegex();

    private sealed class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == oldParameter ? newParameter : base.VisitParameter(node);
    }
}
