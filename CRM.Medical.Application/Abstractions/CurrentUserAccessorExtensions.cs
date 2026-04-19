using CRM.Medical.Application.Exceptions;

namespace CRM.Medical.Application.Abstractions;

public static class CurrentUserAccessorExtensions
{
    public static string GetRequiredUserId(this ICurrentUserAccessor accessor) =>
        accessor.UserId
        ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");
}
