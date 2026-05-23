using System.Linq.Expressions;
using CRM.Medical.Application.Common.Queries;

namespace CRM.Medical.Tests.Common.Queries;

public sealed class QueryFilterExtensionsTests
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<SearchRow, string?>>> TextFields =
        new Dictionary<string, Expression<Func<SearchRow, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = row => row.Name,
            ["notes"] = row => row.Notes
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<SearchRow, bool>>?>> ExactFields =
        new Dictionary<string, Func<string, Expression<Func<SearchRow, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => row => row.Id == value)
        };

    [Fact]
    public void ApplyAdvancedSearch_MatchesDefaultExactNumericPredicate()
    {
        var rows = CreateRows();

        var result = rows
            .AsQueryable()
            .ApplyAdvancedSearch("3", TextFields, ExactFields, BuildDefaultExactPredicate, row => row.Name, row => row.Notes)
            .ToList();

        Assert.Single(result);
        Assert.Equal(3, result[0].Id);
    }

    [Fact]
    public void ApplyAdvancedSearch_MatchesFieldSpecificExactPredicate()
    {
        var rows = CreateRows();

        var result = rows
            .AsQueryable()
            .ApplyAdvancedSearch("id:2", TextFields, ExactFields, BuildDefaultExactPredicate, row => row.Name, row => row.Notes)
            .ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }

    [Fact]
    public void ApplyAdvancedSearch_KeepsTextSearchWorking()
    {
        var rows = CreateRows();

        var result = rows
            .AsQueryable()
            .ApplyAdvancedSearch("beta", TextFields, ExactFields, BuildDefaultExactPredicate, row => row.Name, row => row.Notes)
            .ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }

    private static List<SearchRow> CreateRows() =>
        [
            new(1, "Alpha", "First record"),
            new(2, "Beta", "Second record"),
            new(3, "Gamma", "Third record")
        ];

    private static Expression<Func<SearchRow, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => row => row.Id == value);

    private static Expression<Func<SearchRow, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<SearchRow, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;

    private sealed record SearchRow(int Id, string Name, string? Notes);
}
