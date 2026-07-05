namespace CRM.Medical.Application.Common.Caching;

public static class CacheKeys
{
    private const string UserPrefix = "users";
    public const string QueryResponsePrefix = "queries:";

    public static string UserById(string id) =>
        $"{UserPrefix}:{id}";

    public static string UserList(int page, int pageSize, string? search, bool? isActive, string? role) =>
        $"{UserPrefix}:list:p{page}:ps{pageSize}:s{search ?? "all"}:a{isActive?.ToString() ?? "all"}:r{role ?? "all"}";

    public static string QueryResponse(string requestName, string hash) =>
        $"{QueryResponsePrefix}{requestName}:{hash}";

    public static readonly TimeSpan UserDetailExpiry = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan UserListExpiry = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan QueryResponseExpiry = TimeSpan.FromMinutes(5);
}
