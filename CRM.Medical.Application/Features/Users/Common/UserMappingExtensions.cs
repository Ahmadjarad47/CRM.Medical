using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Application.Mappings;
using CRM.Medical.Domain.Entities;
using Mapster;

namespace CRM.Medical.Application.Features.Users.Common;

internal static class UserMappingExtensions
{
    public static UserSummaryDto ToSummaryDto(this User user) => user.Adapt<UserSummaryDto>();

    public static UserDetailDto ToDetailDto(this User user) => user.Adapt<UserDetailDto>();

    public static UserRolesDto ToRolesDto(this User user, IEnumerable<string> roles)
    {
        var model = new UserRolesMappingModel(user.Id, roles.OrderBy(x => x).ToArray());
        return model.Adapt<UserRolesDto>();
    }

    public static UserPermissionsDto ToPermissionsDto(this User user, IEnumerable<string> permissions)
    {
        var model = new UserPermissionsMappingModel(user.Id, permissions.OrderBy(x => x).ToArray());
        return model.Adapt<UserPermissionsDto>();
    }
}
