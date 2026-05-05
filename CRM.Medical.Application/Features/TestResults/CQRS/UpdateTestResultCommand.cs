using System.Text.Json;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed record UpdateTestResultCommand(
    int Id,
    DateTime ResultDate,
    JsonDocument? ResultData,
    string? PdfUrl,
    string Status) : IRequest<Unit>;
