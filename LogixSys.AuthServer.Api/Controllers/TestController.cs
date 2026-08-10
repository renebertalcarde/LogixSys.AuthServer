using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixSys.AuthServer.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("secure")]
    [Authorize(
        AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Secure()
    {
        return Ok(new
        {
            message = "JWT validation succeeded.",
            authenticated = User.Identity?.IsAuthenticated,
            user = User.Identity?.Name,
            claims = User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })
        });
    }
}