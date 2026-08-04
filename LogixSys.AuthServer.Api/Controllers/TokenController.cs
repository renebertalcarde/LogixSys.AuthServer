using LogixSys.AuthServer.Application.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using AuthSvc = LogixSys.AuthServer.Application.Authentication.IAuthenticationService;

namespace LogixSys.AuthServer.Api.Controllers;

public class TokenController : Controller
{
    private readonly AuthSvc _authenticationService;


    public TokenController(
        AuthSvc authenticationService)
    {
        _authenticationService = authenticationService;
    }


    [HttpPost("token")]
public async Task<IActionResult> Exchange()
{
    var request = HttpContext.GetOpenIddictServerRequest();

    if (request is null)
        return BadRequest();

    if (request.IsAuthorizationCodeGrantType() ||
        request.IsRefreshTokenGrantType())
    {
        var result = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return SignIn(
            result.Principal!,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    return BadRequest();
}
}