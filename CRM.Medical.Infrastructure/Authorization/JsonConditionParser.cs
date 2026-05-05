using System.Collections.Concurrent;
using System.Text.Json;
using CRM.Medical.Application.Authorization;

namespace CRM.Medical.Infrastructure.Authorization;

/// <summary>
/// Minimal JSON condition parser:
/// { "eq": ["user.id", "resource.ownerId"] }
/// { "in": ["Doctor", "user.roles"] }
/// { "all": [ {cond1}, {cond2} ] } / { "any": [ ... ] }
/// </summary>
public sealed class JsonConditionParser : IConditionParser
{
    private readonly ConcurrentDictionary<string, Func<PolicyEvaluationContext, bool>> _cache = new(StringComparer.Ordinal);

    public Func<PolicyEvaluationContext, bool> ParseOrCompile(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return static _ => true;

        return _cache.GetOrAdd(condition, BuildPredicate);
    }

    private static Func<PolicyEvaluationContext, bool> BuildPredicate(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return BuildNode(doc.RootElement);
    }

    private static Func<PolicyEvaluationContext, bool> BuildNode(JsonElement node)
    {
        if (node.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Condition root must be an object.");

        if (node.TryGetProperty("all", out var allNode))
        {
            var items = allNode.EnumerateArray().Select(BuildNode).ToArray();
            return ctx => items.All(x => x(ctx));
        }

        if (node.TryGetProperty("any", out var anyNode))
        {
            var items = anyNode.EnumerateArray().Select(BuildNode).ToArray();
            return ctx => items.Any(x => x(ctx));
        }

        if (node.TryGetProperty("eq", out var eqNode))
        {
            var args = eqNode.EnumerateArray().Select(ReadToken).ToArray();
            if (args.Length != 2)
                throw new InvalidOperationException("'eq' requires exactly 2 arguments.");
            return ctx => string.Equals(Resolve(ctx, args[0]), Resolve(ctx, args[1]), StringComparison.OrdinalIgnoreCase);
        }

        if (node.TryGetProperty("neq", out var neqNode))
        {
            var args = neqNode.EnumerateArray().Select(ReadToken).ToArray();
            if (args.Length != 2)
                throw new InvalidOperationException("'neq' requires exactly 2 arguments.");
            return ctx => !string.Equals(Resolve(ctx, args[0]), Resolve(ctx, args[1]), StringComparison.OrdinalIgnoreCase);
        }

        if (node.TryGetProperty("in", out var inNode))
        {
            var args = inNode.EnumerateArray().Select(ReadToken).ToArray();
            if (args.Length != 2)
                throw new InvalidOperationException("'in' requires exactly 2 arguments.");

            return ctx =>
            {
                var left = Resolve(ctx, args[0]);
                var right = args[1];
                if (string.Equals(right, "user.roles", StringComparison.OrdinalIgnoreCase))
                    return ctx.Roles.Any(r => string.Equals(r, left, StringComparison.OrdinalIgnoreCase));

                return string.Equals(left, Resolve(ctx, right), StringComparison.OrdinalIgnoreCase);
            };
        }

        throw new InvalidOperationException("Unsupported condition operator.");
    }

    private static string ReadToken(JsonElement item) => item.ValueKind switch
    {
        JsonValueKind.String => item.GetString() ?? string.Empty,
        JsonValueKind.Number => item.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        _ => throw new InvalidOperationException("Condition arguments must be primitive values.")
    };

    private static string Resolve(PolicyEvaluationContext ctx, string token)
    {
        if (token.StartsWith(AuthorizationConstants.UserContextPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var key = token[AuthorizationConstants.UserContextPrefix.Length..];
            return key.Equals("id", StringComparison.OrdinalIgnoreCase)
                ? ctx.UserId
                : string.Empty;
        }

        if (token.StartsWith(AuthorizationConstants.ResourceContextPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var key = token[AuthorizationConstants.ResourceContextPrefix.Length..];
            return ctx.Resource.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
        }

        if (token.StartsWith(AuthorizationConstants.RequestContextPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var key = token[AuthorizationConstants.RequestContextPrefix.Length..];
            return ctx.Request.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
        }

        return token;
    }
}
