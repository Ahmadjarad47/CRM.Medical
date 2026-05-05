using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record ListAccessPoliciesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<PagedResult<AccessPolicyDto>>;
