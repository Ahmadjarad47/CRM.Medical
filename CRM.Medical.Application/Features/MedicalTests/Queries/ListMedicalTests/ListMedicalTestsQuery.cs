using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.ListMedicalTests;

public sealed record ListMedicalTestsQuery(
    int Page,
    int PageSize,
    string? Category,
    string? Status) : IRequest<PagedResult<MedicalTestDto>>;
