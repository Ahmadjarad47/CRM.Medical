using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.WelcomePages;

internal static class WelcomePageMappings
{
    public static WelcomePageDto ToDto(this WelcomePage welcomePage) =>
        new(
            welcomePage.Id,
            welcomePage.Name,
            welcomePage.Description,
            welcomePage.MediaType,
            welcomePage.MediaUrl,
            welcomePage.IsActive,
            welcomePage.CreatedAt,
            welcomePage.UpdatedAt);
}
