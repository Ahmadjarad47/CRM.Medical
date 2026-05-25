using CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.User;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Doctor + "," + UserRoles.Patient + "," + UserRoles.LabPartner)]
public sealed class DashboardController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public Task<DashboardResponse> Get(CancellationToken cancellationToken) =>
        mediator.Send(new GetDashboardQuery(), cancellationToken);
}
