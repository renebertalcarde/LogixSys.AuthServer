using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace LogixSys.AuthServer.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("secure")]
    [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Secure()
    {
        return Ok(new
        {
            authenticated = User.Identity?.IsAuthenticated,
            name = User.Identity?.Name,
            userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(
                ClaimTypes.Email),

            roles = User.FindAll(
                    ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct(),

            claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            })
        });
    }

    [HttpGet("admin")]
    [Authorize(
    AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme,
    Roles = "admin")]
    public IActionResult Admin()
    {
        return Ok(new
        {
            authenticated = User.Identity?.IsAuthenticated,
            name = User.Identity?.Name,
            userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(
                ClaimTypes.Email),

            roles = User.FindAll(
                    ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct(),

            claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            })
        });
    }
}