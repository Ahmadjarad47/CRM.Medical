using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalWorkflow;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

public sealed class TestResultService(MedicalDbContext db, ICurrentUserAccessor currentUser, IAccessPolicyEvaluator accessPolicyEvaluator)
    : ITestResultService
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<TestResult, string?>>> SearchFields =
        new Dictionary<string, Expression<Func<TestResult, string?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["status"] = r => r.Status,
            ["pdf"] = r => r.PdfUrl
        };

    public async Task<PagedResult<TestResultDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        int? testRequestId,
        CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var scopedRequests = await accessPolicyEvaluator.ApplyAsync(db.TestRequests.AsNoTracking(), "test_requests", "read", cancellationToken);
        var query =
            from result in db.TestResults.AsNoTracking()
            join tr in scopedRequests on result.TestRequestId equals tr.Id
            select result;

        if (testRequestId is { } tid)
            query = query.Where(r => r.TestRequestId == tid);

        query = query.ApplyAdvancedSearch(search, SearchFields, r => r.Status, r => r.PdfUrl);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await (
            from result in query
            join tr in scopedRequests on result.TestRequestId equals tr.Id
            join createdBy in db.Users.AsNoTracking() on tr.CreatedByUserId equals createdBy.Id into createdByUsers
            from createdBy in createdByUsers.DefaultIfEmpty()
            orderby result.ResultDate descending
            select new
            {
                Result = result,
                TestRequestCreatedByUserId = tr.CreatedByUserId,
                TestRequestCreatedByFullName = createdBy == null ? null : createdBy.FullName
            })
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TestResultDto>
        {
            Items = rows
                .Select(r => Map(r.Result, r.TestRequestCreatedByUserId, r.TestRequestCreatedByFullName))
                .ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TestResultDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var entity = await db.TestResults.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        var canRead = await accessPolicyEvaluator.CanAccessAsync(request, "test_requests", "read", cancellationToken);
        if (!canRead)
            throw new ApplicationForbiddenException("You cannot access this test request.");

        var createdByFullName = await GetUserFullNameAsync(request.CreatedByUserId, cancellationToken);
        return Map(entity, request.CreatedByUserId, createdByFullName);
    }

    public async Task<TestResultDto> GetByTestRequestIdAsync(int testRequestId, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var request = await db.TestRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        var canRead = await accessPolicyEvaluator.CanAccessAsync(request, "test_requests", "read", cancellationToken);
        if (!canRead)
            throw new ApplicationForbiddenException("You cannot access this test request.");

        var entity = await db.TestResults.AsNoTracking().FirstOrDefaultAsync(r => r.TestRequestId == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException("No result exists for this test request.");

        var createdByFullName = await GetUserFullNameAsync(request.CreatedByUserId, cancellationToken);
        return Map(entity, request.CreatedByUserId, createdByFullName);
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

        var request = await db.TestRequests.FirstOrDefaultAsync(r => r.Id == testRequestId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{testRequestId}' was not found.");

        var canCreate = await accessPolicyEvaluator.CanAccessAsync(request, "test_results", "create", cancellationToken);
        if (!canCreate)
            throw new ApplicationForbiddenException("You cannot create this test result.");

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

        var createdByFullName = await GetUserFullNameAsync(request.CreatedByUserId, cancellationToken);
        return Map(entity, request.CreatedByUserId, createdByFullName);
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

        var entity = await db.TestResults.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        var canUpdate = await accessPolicyEvaluator.CanAccessAsync(request, "test_results", "update", cancellationToken);
        if (!canUpdate)
            throw new ApplicationForbiddenException("You cannot modify this test result.");

        entity.ResultDate = resultDate;
        entity.ResultData = resultData;
        entity.PdfUrl = string.IsNullOrWhiteSpace(pdfUrl) ? null : pdfUrl.Trim();
        entity.Status = status.Trim();

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        MedicalWorkflowAuthorization.RequireAuthenticatedUser(currentUser);

        var entity = await db.TestResults.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{id}' was not found.");

        var request = await db.TestRequests.AsNoTracking().FirstAsync(r => r.Id == entity.TestRequestId, cancellationToken);
        var canDelete = await accessPolicyEvaluator.CanAccessAsync(request, "test_results", "delete", cancellationToken);
        if (!canDelete)
            throw new ApplicationForbiddenException("You cannot delete this test result.");

        db.TestResults.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    private Task<string?> GetUserFullNameAsync(string? userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Task.FromResult<string?>(null);

        return db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static TestResultDto Map(
        TestResult e,
        string? testRequestCreatedByUserId,
        string? testRequestCreatedByFullName) =>
        new(
            e.Id,
            e.TestRequestId,
            testRequestCreatedByUserId,
            testRequestCreatedByFullName,
            testRequestCreatedByFullName,
            e.ResultDate,
            MedicalWorkflowJson.ToJsonElement(e.ResultData),
            e.PdfUrl,
            e.Status,
            e.CreatedAt,
            e.UpdatedAt);
}
