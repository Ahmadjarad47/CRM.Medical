using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class MedicalTestService(MedicalDbContext db, ICurrentUserAccessor user)
    : IMedicalTestService
{
    public async Task<IReadOnlyList<MedicalTestDto>> ListAsync(CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(user, UserPermissions.MedicalTestRead);

        var items = await db.MedicalTests
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<MedicalTestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(user, UserPermissions.MedicalTestRead);

        var entity = await db.MedicalTests.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");

        return Map(entity);
    }

    public async Task<MedicalTestDto> CreateAsync(
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(user, UserPermissions.MedicalTestCreate);
        MedicalWorkflowAuthorization.DenyPatientMutations(user);

        var userId = user.GetRequiredUserId();
        var entity = new MedicalTest
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Price = price,
            Category = category.Trim(),
            SampleType = sampleType.Trim(),
            ParameterSchema = parameterSchema,
            Status = status.Trim(),
            CreatedByUserId = userId
        };

        db.MedicalTests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(user, UserPermissions.MedicalTestUpdate);
        MedicalWorkflowAuthorization.DenyPatientMutations(user);

        var entity = await db.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");

        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Price = price;
        entity.Category = category.Trim();
        entity.SampleType = sampleType.Trim();
        entity.ParameterSchema = parameterSchema;
        entity.Status = status.Trim();

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(user);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(user, UserPermissions.MedicalTestDelete);
        MedicalWorkflowAuthorization.DenyPatientMutations(user);

        var entity = await db.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{id}' was not found.");

        var inUse = await db.TestRequests.AnyAsync(r => r.MedicalTestId == id, cancellationToken);
        if (inUse)
            throw new ApplicationConflictException("Cannot delete a medical test that has test requests.");

        db.MedicalTests.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static MedicalTestDto Map(MedicalTest e) =>
        new(
            e.Id,
            e.NameAr,
            e.NameEn,
            e.Price,
            e.Category,
            e.SampleType,
            MedicalWorkflowJson.ToJsonElement(e.ParameterSchema),
            e.Status,
            e.CreatedAt,
            e.UpdatedAt);
}
