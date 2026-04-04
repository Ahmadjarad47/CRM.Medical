using CRM.Medical.API.Controllers.Admin.Models;
using CRM.Medical.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;
using CRM.Medical.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using CRM.Medical.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;
using CRM.Medical.Application.Features.AppointmentTypes.Queries.ListAdminAppointmentTypes;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/appointment-types")]
public sealed class AdminAppointmentTypesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.AppointmentsView)]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAll(CancellationToken ct) =>
        Ok(await mediator.Send(new ListAdminAppointmentTypesQuery(), ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.AppointmentsView)]
    [ProducesResponseType(typeof(AppointmentTypeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetAppointmentTypeByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.AppointmentsManage)]
    [ProducesResponseType(typeof(AppointmentTypeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentTypeRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(new CreateAppointmentTypeCommand(request.Name), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.AppointmentsManage)]
    [ProducesResponseType(typeof(AppointmentTypeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentTypeRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new UpdateAppointmentTypeCommand(id, request.Name, request.IsActive), ct));
}
