using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null,
    string? Role = null,
    string SortBy = "FullName",
    bool SortDescending = false)
    : IRequest<PagedResult<UserSummaryDto>>;
