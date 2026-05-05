namespace CRM.Medical.API.Contracts.Common;

public class PagedSearchRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
}
