using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.System;

[AllowAnonymous]
[Route("/")]
public sealed class RootController : SystemBaseController
{
    [HttpGet]
    public IActionResult Index() => Redirect("/swagger");
}
