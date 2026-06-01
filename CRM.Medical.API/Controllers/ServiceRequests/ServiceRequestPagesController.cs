using CRM.Medical.API.Contracts.ServiceRequests;
using CRM.Medical.Application.Features.ServiceRequests.CQRS;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.ServiceRequests;

[ApiController]
[Route("api/service-request-pages")]
public sealed class ServiceRequestPagesController(ISender mediator) : ControllerBase
{
    [HttpGet("{pageType}")]
    [ProducesResponseType(typeof(ServiceRequestPageSettingDto), StatusCodes.Status200OK)]
    public Task<ServiceRequestPageSettingDto> GetPublic(ServiceRequestPageType pageType, CancellationToken ct) =>
        mediator.Send(new GetServiceRequestPageSettingQuery(pageType), ct);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceRequestPageSettingDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ServiceRequestPageSettingDto>> List(CancellationToken ct) =>
        mediator.Send(new ListServiceRequestPageSettingsQuery(), ct);

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ServiceRequestPageSettingDto), StatusCodes.Status200OK)]
    public Task<ServiceRequestPageSettingDto> Update(
        int id,
        [FromBody] UpdateServiceRequestPageSettingRequest request,
        CancellationToken ct) =>
        mediator.Send(new UpdateServiceRequestPageSettingCommand(
            id,
            request.AnnouncementTextAr,
            request.AnnouncementTextEn,
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.IsActive), ct);
}
