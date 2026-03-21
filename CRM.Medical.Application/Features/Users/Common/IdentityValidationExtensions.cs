using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Common;

internal static class IdentityValidationExtensions
{
    public static void ThrowIfFailed(this IdentityResult result, string propertyName)
    {
        if (result.Succeeded)
            return;

        var failures = result.Errors
            .Select(error => new ValidationFailure(propertyName, error.Description))
            .ToArray();

        throw new ValidationException(failures);
    }
}
