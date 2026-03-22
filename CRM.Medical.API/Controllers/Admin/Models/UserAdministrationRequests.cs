using CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.AssignRoles;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.LockUser;
using CRM.Medical.Application.Features.Users.Commands.RemoveRoles;
using CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;
using CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPassword;
using CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed record ManageRolesRequest(IReadOnlyCollection<string> Roles);

public sealed record ManagePermissionsRequest(IReadOnlyCollection<string> Permissions);

public sealed record LockUserRequest(int LockoutMinutes);

public sealed record SendEmailVerificationRequest(string ConfirmationBaseUrl);

public sealed record UpdateUserPasswordRequest(string CurrentPassword, string NewPassword);

public sealed record CreateUserRequest(
    string DisplayName,
    string Email,
    string Password,
    UserType UserType,
    DateOnly? DateOfBirth,
    string? PhoneSecondary,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country,
    string? NationalIdNumber,
    string? InsuranceProvider,
    string? InsurancePolicyNumber,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalLicenseNumber,
    string? Specialty,
    string? ClinicName,
    string? LabName,
    string? LabLicenseNumber,
    string? LabContactName,
    string? LabContactPhone,
    string? SpecialAccountCode,
    string? SpecialNotes);

public sealed record UpdateUserRequest(
    string DisplayName,
    string Email,
    UserType UserType,
    DateOnly? DateOfBirth,
    string? PhoneSecondary,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country,
    string? NationalIdNumber,
    string? InsuranceProvider,
    string? InsurancePolicyNumber,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalLicenseNumber,
    string? Specialty,
    string? ClinicName,
    string? LabName,
    string? LabLicenseNumber,
    string? LabContactName,
    string? LabContactPhone,
    string? SpecialAccountCode,
    string? SpecialNotes);

public static class UserAdministrationRequestMappings
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request) =>
        new(
            request.DisplayName,
            request.Email,
            request.Password,
            request.UserType,
            request.DateOfBirth,
            request.PhoneSecondary,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.Region,
            request.PostalCode,
            request.Country,
            request.NationalIdNumber,
            request.InsuranceProvider,
            request.InsurancePolicyNumber,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MedicalLicenseNumber,
            request.Specialty,
            request.ClinicName,
            request.LabName,
            request.LabLicenseNumber,
            request.LabContactName,
            request.LabContactPhone,
            request.SpecialAccountCode,
            request.SpecialNotes);

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, string userId) =>
        new(
            userId,
            request.DisplayName,
            request.Email,
            request.UserType,
            request.DateOfBirth,
            request.PhoneSecondary,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.Region,
            request.PostalCode,
            request.Country,
            request.NationalIdNumber,
            request.InsuranceProvider,
            request.InsurancePolicyNumber,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MedicalLicenseNumber,
            request.Specialty,
            request.ClinicName,
            request.LabName,
            request.LabLicenseNumber,
            request.LabContactName,
            request.LabContactPhone,
            request.SpecialAccountCode,
            request.SpecialNotes);

    public static UpdateUserPasswordCommand ToCommand(this UpdateUserPasswordRequest request, string userId) =>
        new(userId, request.CurrentPassword, request.NewPassword);

    public static AssignRolesCommand ToAssignRolesCommand(this ManageRolesRequest request, string userId) =>
        new(userId, request.Roles);

    public static RemoveRolesCommand ToRemoveRolesCommand(this ManageRolesRequest request, string userId) =>
        new(userId, request.Roles);

    public static AddUserPermissionsCommand ToAddPermissionsCommand(
        this ManagePermissionsRequest request,
        string userId) =>
        new(userId, request.Permissions);

    public static UpdateUserPermissionsCommand ToUpdatePermissionsCommand(
        this ManagePermissionsRequest request,
        string userId) =>
        new(userId, request.Permissions);

    public static RemoveUserPermissionsCommand ToRemovePermissionsCommand(
        this ManagePermissionsRequest request,
        string userId) =>
        new(userId, request.Permissions);

    public static LockUserCommand ToCommand(this LockUserRequest request, string userId) =>
        new(userId, request.LockoutMinutes);

    public static SendEmailVerificationCommand ToCommand(
        this SendEmailVerificationRequest request,
        string userId) =>
        new(userId, request.ConfirmationBaseUrl);
}
