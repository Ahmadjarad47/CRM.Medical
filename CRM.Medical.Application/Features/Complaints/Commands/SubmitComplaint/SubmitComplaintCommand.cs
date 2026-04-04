using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;

public sealed record SubmitComplaintCommand(
    string UserId,
    string Title,
    string Description,
    byte[]? FileBytes,
    string? ContentType,
    string? FileName) : IRequest<ComplaintDto>;
