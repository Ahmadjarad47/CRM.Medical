using CRM.Medical.API.Controllers.Admin.Models;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.SubscriptionPackages.Commands.CreateSubscriptionPackage;
using CRM.Medical.Application.Features.SubscriptionPackages.Commands.SetSubscriptionPackageActive;
using CRM.Medical.Application.Features.SubscriptionPackages.Commands.UpdateSubscriptionPackage;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using CRM.Medical.Application.Features.SubscriptionPackages.Queries.GetSubscriptionPackageById;
using CRM.Medical.Application.Features.SubscriptionPackages.Queries.ListSubscriptionPackages;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/subscription-packages")]
public sealed class SubscriptionPackagesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.SubscriptionsView)]
    [ProducesResponseType(typeof(PagedResult<SubscriptionPackageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListSubscriptionPackagesRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListSubscriptionPackagesQuery(
                request.Page,
                request.PageSize,
                request.TargetAudience,
                request.IsActive),
            ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.SubscriptionsView)]
    [ProducesResponseType(typeof(SubscriptionPackageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetSubscriptionPackageByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.SubscriptionsManage)]
    [ProducesResponseType(typeof(SubscriptionPackageDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionPackageRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(
            new CreateSubscriptionPackageCommand(
                request.Name,
                request.Price,
                request.ValidityDays,
                request.IncludedTests,
                request.TargetAudience,
                request.IsActive),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.SubscriptionsManage)]
    [ProducesResponseType(typeof(SubscriptionPackageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubscriptionPackageRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateSubscriptionPackageCommand(
                id,
                request.Name,
                request.Price,
                request.ValidityDays,
                request.IncludedTests,
                request.TargetAudience,
                request.IsActive),
            ct));

    [HttpPost("{id:int}/activate")]
    [Authorize(Policy = UserPermissions.SubscriptionsManage)]
    [ProducesResponseType(typeof(SubscriptionPackageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activate(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new SetSubscriptionPackageActiveCommand(id, true), ct));

    [HttpPost("{id:int}/deactivate")]
    [Authorize(Policy = UserPermissions.SubscriptionsManage)]
    [ProducesResponseType(typeof(SubscriptionPackageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new SetSubscriptionPackageActiveCommand(id, false), ct));
}
