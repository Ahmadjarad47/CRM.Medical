using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.User;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class UserBaseController : ControllerBase;
