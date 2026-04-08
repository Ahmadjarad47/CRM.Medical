using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Queries.GetTestRequestById;

public sealed record GetTestRequestByIdQuery(int Id) : IRequest<TestRequestDto>;
