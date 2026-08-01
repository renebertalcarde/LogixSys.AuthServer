using Microsoft.AspNetCore.Mvc;

namespace LogixSys.AuthServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}