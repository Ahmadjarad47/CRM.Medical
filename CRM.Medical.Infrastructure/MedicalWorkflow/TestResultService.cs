using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class TestResultService(MedicalDbContext db, ICurrentUserAccessor currentUser)
    : ITestResultService
{
    private readonly TestRequestAccessEvaluator _access = new(db, currentUser);

    public async Task<IReadOnlyList<TestResultDto>> ListAsync(
        int? testRequestId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultRead);

        var scopedRequests = _access.FilterAccessible(db.TestRequests.AsNoTracking());
        var query =
            from result in db.TestResults.AsNoTracking()
            join tr in scopedRequests on result.TestRequestId equals tr.Id
            select result;

        if (testRequestId is { } tid)
            query = query.Where(r => r.TestRequestId == tid);

        var rows = await query.OrderByDescending(r => r.ResultDate).ToListAsync(cancellationToken);
        return rows.Select(Map).ToList();
    }

    public async Task<TestResultDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultRead);

        var entity = await db.TestResults.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        await _access.EnsureCanAccessAsync(request, cancellationToken);

        return Map(entity);
    }

    public async Task<TestResultDto> GetByTestRequestIdAsync(int testRequestId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultRead);

        var request = await db.TestRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        await _access.EnsureCanAccessAsync(request, cancellationToken);

        var entity = await db.TestResults.AsNoTracking().FirstOrDefaultAsync(r => r.TestRequestId == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException("No result exists for this test request.");

        return Map(entity);
    }

    public async Task<TestResultDto> CreateAsync(
        int testRequestId,
        DateTime resultDate,
        JsonDocument? resultData,
        string? pdfUrl,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultCreate);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var request = await db.TestRequests.FirstOrDefaultAsync(r => r.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        await _access.EnsureCanAccessAsync(request, cancellationToken);

        var exists = await db.TestResults.AnyAsync(r => r.TestRequestId == testRequestId, cancellationToken);
        if (exists)
            throw new ApplicationConflictException("A result already exists for this test request.");

        var userId = currentUser.GetRequiredUserId();
        var entity = new TestResult
        {
            TestRequestId = testRequestId,
            ResultDate = resultDate,
            ResultData = resultData,
            PdfUrl = string.IsNullOrWhiteSpace(pdfUrl) ? null : pdfUrl.Trim(),
            Status = status.Trim(),
            CreatedByUserId = userId
        };

        db.TestResults.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task UpdateAsync(
        int id,
        DateTime resultDate,
        JsonDocument? resultData,
        string? pdfUrl,
        string status,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultUpdate);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestResults.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        await _access.EnsureCanAccessAsync(request, cancellationToken);

        entity.ResultDate = resultDate;
        entity.ResultData = resultData;
        entity.PdfUrl = string.IsNullOrWhiteSpace(pdfUrl) ? null : pdfUrl.Trim();
        entity.Status = status.Trim();

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);
        MedicalWorkflowAuthorization.RequirePermissionOrAdmin(currentUser, UserPermissions.TestResultDelete);
        MedicalWorkflowAuthorization.DenyPatientMutations(currentUser);

        var entity = await db.TestResults.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        await _access.EnsureCanAccessAsync(request, cancellationToken);

        db.TestResults.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static TestResultDto Map(TestResult e) =>
        new(
            e.Id,
            e.TestRequestId,
            e.ResultDate,
            MedicalWorkflowJson.ToJsonElement(e.ResultData),
            e.PdfUrl,
            e.Status,
            e.CreatedAt,
            e.UpdatedAt);
}
