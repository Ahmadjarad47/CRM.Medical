using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string UserId,
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
    string? SpecialNotes) : IRequest<UserDetailDto>;
