using CRM.Medical.Application.Features.Complaints.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;

public sealed record SubmitComplaintCommand(
    string UserId,
    string Title,
    string Description,
    IFormFile? Attachment) : IRequest<ComplaintDto>;
