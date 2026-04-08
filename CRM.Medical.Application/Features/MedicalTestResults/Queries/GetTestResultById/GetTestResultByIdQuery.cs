using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Queries.GetTestResultById;

public sealed record GetTestResultByIdQuery(int Id) : IRequest<TestResultDto>;
