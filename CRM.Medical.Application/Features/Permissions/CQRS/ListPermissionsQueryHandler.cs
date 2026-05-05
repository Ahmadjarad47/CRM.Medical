using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class ListAccessPoliciesQueryHandler(IAccessPolicyService accessPolicyService)
    : IRequestHandler<ListAccessPoliciesQuery, PagedResult<AccessPolicyDto>>
{
    public Task<PagedResult<AccessPolicyDto>> Handle(
        ListAccessPoliciesQuery request,
        CancellationToken cancellationToken) =>
        accessPolicyService.ListAsync(request.Page, request.PageSize, request.Search, cancellationToken);
}
