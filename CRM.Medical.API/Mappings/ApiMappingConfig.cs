using CRM.Medical.API.Controllers.Admin;
using CRM.Medical.API.Controllers.User;
using CRM.Medical.Application.Features.Auth.Login;
using CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;
using CRM.Medical.Application.Features.Users.Commands.LockUser;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;
using Mapster;

namespace CRM.Medical.API.Mappings;

public sealed class ApiMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuthController.LoginRequest, LoginCommand>();
        config.NewConfig<EmailVerificationController.ConfirmEmailRequest, ConfirmEmailCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);

        config.NewConfig<UserAdministrationController.UpdateUserRequest, UpdateUserCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.UpdateUserPasswordRequest, UpdateUserPasswordCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.ManageRolesRequest, AssignRolesCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.ManageRolesRequest, RemoveRolesCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.ManagePermissionsRequest, AddUserPermissionsCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.ManagePermissionsRequest, UpdateUserPermissionsCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.ManagePermissionsRequest, RemoveUserPermissionsCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.LockUserRequest, LockUserCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
        config.NewConfig<UserAdministrationController.SendEmailVerificationRequest, SendEmailVerificationCommand>()
            .Map(dest => dest.UserId, _ => string.Empty);
    }
}
