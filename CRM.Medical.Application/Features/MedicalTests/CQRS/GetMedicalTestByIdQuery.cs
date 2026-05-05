using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record GetMedicalTestByIdQuery(int Id) : IRequest<MedicalTestDto>;
