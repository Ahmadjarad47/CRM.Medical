using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.GetMedicalTestById;

public sealed record GetMedicalTestByIdQuery(int Id) : IRequest<MedicalTestDto>;
