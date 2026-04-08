using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.CreateMedicalTest;

public sealed record CreateMedicalTestCommand(
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status,
    string CreatedByUserId) : IRequest<MedicalTestDto>;
