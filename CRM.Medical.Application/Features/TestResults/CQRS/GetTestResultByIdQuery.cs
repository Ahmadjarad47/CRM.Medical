using CRM.Medical.Application.Features.TestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed record GetTestResultByIdQuery(int Id) : IRequest<TestResultDto>;
