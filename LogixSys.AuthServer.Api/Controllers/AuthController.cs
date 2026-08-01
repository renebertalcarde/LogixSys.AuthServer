using LogixSys.AuthServer.Application.Authentication;
using LogixSys.AuthServer.Contracts.Requests;
using LogixSys.AuthServer.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LogixSys.AuthServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;


    public AuthController(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }


    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authenticationService.AuthenticateAsync(
                request.Username,
                request.Password,
                cancellationToken);


        if (!result.Success)
        {
            return Unauthorized(
                new LoginResponse
                {
                    Success = false,
                    Message = result.Error
                });
        }


        return Ok(
            new LoginResponse
            {
                Success = true,
                UserId = result.UserId,
                UserName = result.UserName,
                Roles = result.Roles,
                Message = "Authentication successful."
            });
    }
}