using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixSys.AuthServer.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestApiController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new
        {
            message = "Public endpoint works."
        });
    }

    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok(new
        {
            message = "JWT validation succeeded.",
            user = User.Identity?.Name,
            authenticated = User.Identity?.IsAuthenticated,
            claims = User.Claims.Select(c => new
            {
                type = c.Type,
                value = c.Value
            })
        });
    }

    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "Administrator")]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok(new
        {
            message = "JWT validation and Administrator role authorization succeeded.",
            user = User.Identity?.Name
        });
    }
}