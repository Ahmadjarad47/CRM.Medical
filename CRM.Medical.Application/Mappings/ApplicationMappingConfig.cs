using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using Mapster;

namespace CRM.Medical.Application.Mappings;

public sealed class ApplicationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserSummaryDto>()
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(
                dest => dest.IsLocked,
                src => src.LockoutEnd.HasValue && src.LockoutEnd.Value.ToUniversalTime() > DateTimeOffset.UtcNow);

        config.NewConfig<User, UserDetailDto>()
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(
                dest => dest.LockoutEnd,
                src => src.LockoutEnd.HasValue ? src.LockoutEnd.Value.ToUniversalTime() : (DateTimeOffset?)null)
            .Map(
                dest => dest.IsLocked,
                src => src.LockoutEnd.HasValue && src.LockoutEnd.Value.ToUniversalTime() > DateTimeOffset.UtcNow);

        config.NewConfig<UserRolesMappingModel, UserRolesDto>();
        config.NewConfig<UserPermissionsMappingModel, UserPermissionsDto>();
        config.NewConfig<LoginResponseMappingModel, LoginResponse>();
    }
}
