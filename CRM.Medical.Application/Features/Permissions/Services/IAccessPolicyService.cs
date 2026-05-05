using System.Text.Json;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IAccessPolicyService
{
    Task<AccessPolicyDto> CreateAsync(
        string name,
        string resource,
        string action,
        SubjectType subjectType,
        string subjectId,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isActive,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Guid id,
        string name,
        string resource,
        string action,
        SubjectType subjectType,
        string subjectId,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<AccessPolicyDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);
}
