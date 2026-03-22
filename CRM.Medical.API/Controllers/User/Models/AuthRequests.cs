using CRM.Medical.Application.Features.Auth.ForgotPassword;
using CRM.Medical.Application.Features.Auth.Login;
using CRM.Medical.Application.Features.Auth.RefreshToken;
using CRM.Medical.Application.Features.Auth.ResendEmailVerification;
using CRM.Medical.Application.Features.Auth.ResetPassword;
using CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;
using CRM.Medical.Application.Features.Users.Commands.CreateUser;
using CRM.Medical.Application.Features.Users.Commands.UpdateUser;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Controllers.User.Models;

public sealed record CreateAccountRequest(
    string DisplayName,
    string Email,
    string Password,
    DateOnly? DateOfBirth,
    string ConfirmationBaseUrl,
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
    string? EmergencyContactPhone);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string UserId, string RefreshToken);

public sealed record ConfirmEmailRequest(string UserId, string Token);

public sealed record ResendEmailVerificationRequest(string Email, string ConfirmationBaseUrl);

public sealed record ForgotPasswordRequest(string Email, string ResetBaseUrl);

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

public sealed record UpdateProfileRequest(
    string DisplayName,
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

public static class AuthRequestMappings
{
    public static CreateUserCommand ToCreateAccountCommand(this CreateAccountRequest request) =>
        new(
            request.DisplayName,
            request.Email,
            request.Password,
            UserType.Patient,
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
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request) =>
        new(request.UserId, request.RefreshToken);

    public static ConfirmEmailCommand ToCommand(this ConfirmEmailRequest request) =>
        new(request.UserId, request.Token);

    public static ResendEmailVerificationCommand ToCommand(this ResendEmailVerificationRequest request) =>
        new(request.Email, request.ConfirmationBaseUrl);

    public static ForgotPasswordCommand ToCommand(this ForgotPasswordRequest request) =>
        new(request.Email, request.ResetBaseUrl);

    public static ResetPasswordCommand ToCommand(this ResetPasswordRequest request) =>
        new(request.Email, request.Token, request.NewPassword);

    public static UpdateUserCommand ToUpdateProfileCommand(
        this UpdateProfileRequest request,
        string userId,
        UserDetailDto current) =>
        new(
            userId,
            request.DisplayName,
            current.Email,
            current.UserType,
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
}
