using CRM.Medical.API.Contracts.MedicalWorkflow.AppointmentMedicalTest;
using CRM.Medical.Application.Features.Appointments.Commands.UpdateAppointmentMedicalTest;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.MedicalWorkflow;

[ApiController]
[Authorize]
[Route("api/medical-workflow/appointments")]
public sealed class AppointmentMedicalTestController(ISender mediator) : ControllerBase
{
    [HttpPut("{appointmentId:int}/medical-test")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLink(
        int appointmentId,
        [FromBody] UpdateAppointmentMedicalTestRequest request,
        CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateAppointmentMedicalTestCommand(
                appointmentId,
                request.MedicalTestId,
                request.MedicalTestCompletionStatus),
            ct));
}
