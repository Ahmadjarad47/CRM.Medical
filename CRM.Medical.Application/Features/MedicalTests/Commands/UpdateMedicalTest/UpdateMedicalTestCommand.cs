using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.UpdateMedicalTest;

public sealed record UpdateMedicalTestCommand(
    int Id,
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status) : IRequest<MedicalTestDto>;
