using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class ExternalPatientService(
    MedicalDbContext db,
    ICurrentUserAccessor currentUser,
    UserManager<User> userManager,
    IAccessPolicyEvaluator accessPolicyEvaluator) : IExternalPatientService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<ExternalPatient, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<ExternalPatient, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = e => e.FullName,
            ["phone"] = e => e.PhoneNumber,
            ["externalid"] = e => e.ExternalId,
            ["gender"] = e => e.Gender
        };

    private static readonly IReadOnlyDictionary<string, Func<string, Expression<Func<ExternalPatient, bool>>?>> ExactSearchFields =
        new Dictionary<string, Func<string, Expression<Func<ExternalPatient, bool>>?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = token => ParseIntPredicate(token, value => e => e.Id == value),
            ["age"] = token => ParseIntPredicate(token, value => e => e.Age == value)
        };

    public async Task<PagedResult<ExternalPatientDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = await accessPolicyEvaluator.ApplyAsync(db.ExternalPatients.AsNoTracking(), "external_patients", "read", cancellationToken);

        query = query.ApplyAdvancedSearch(
            search,
            SearchFields,
            ExactSearchFields,
            BuildDefaultExactPredicate,
            e => e.FullName,
            e => e.PhoneNumber,
            e => e.ExternalId,
            e => e.Gender);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ExternalPatientDto>
        {
            Items = rows.Select(Map).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ExternalPatientDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var entity = await (await accessPolicyEvaluator.ApplyAsync(db.ExternalPatients.AsNoTracking(), "external_patients", "read", cancellationToken))
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"External patient '{id}' was not found.");

        return Map(entity);
    }

    public async Task<ExternalPatientDto> CreateAsync(
        string fullName,
        int? age,
        string gender,
        string phoneNumber,
        string? externalId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ApplicationBadRequestException("Full name is required.");
        if (string.IsNullOrWhiteSpace(gender))
            throw new ApplicationBadRequestException("Gender is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ApplicationBadRequestException("Phone number is required.");

        var entity = new ExternalPatient
        {
            FullName = fullName.Trim(),
            Age = age,
            Gender = gender.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim(),
            CreatedByUserId = currentUser.GetRequiredUserId()
        };

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(entity, "external_patients", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this external patient.");

        db.ExternalPatients.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task LinkToDirectPatientAsync(int externalPatientId, string directPatientUserId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        if (string.IsNullOrWhiteSpace(directPatientUserId))
            throw new ApplicationBadRequestException("DirectPatientUserId is required.");

        directPatientUserId = directPatientUserId.Trim();

        var patient = await userManager.FindByIdAsync(directPatientUserId)
            ?? throw new ApplicationBadRequestException("Direct patient was not found.");

        if (!await userManager.IsInRoleAsync(patient, UserRoles.Patient))
            throw new ApplicationBadRequestException("The user must be a patient account.");

        var entity = await db.ExternalPatients.FirstOrDefaultAsync(e => e.Id == externalPatientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"External patient '{externalPatientId}' was not found.");

        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(entity, "external_patients", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot update this external patient.");

        entity.LinkToDirectPatient(directPatientUserId);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static ExternalPatientDto Map(ExternalPatient e) =>
        new(e.Id, e.FullName, e.Age, e.Gender, e.PhoneNumber, e.ExternalId, e.LinkedDirectPatientId, e.CreatedAt);

    private static Expression<Func<ExternalPatient, bool>>? BuildDefaultExactPredicate(string token) =>
        ParseIntPredicate(token, value => e => e.Id == value || e.Age == value);

    private static Expression<Func<ExternalPatient, bool>>? ParseIntPredicate(
        string token,
        Func<int, Expression<Func<ExternalPatient, bool>>> predicateFactory) =>
        int.TryParse(token, out var value) ? predicateFactory(value) : null;
}
