using LogixSys.AuthServer.Application.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace LogixSys.AuthServer.Api.Controllers;

public class TokenController : Controller
{
    private readonly IAuthenticationService _authenticationService;


    public TokenController(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }


    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange(
        CancellationToken cancellationToken)
    {
        var request =
            HttpContext.GetOpenIddictServerRequest();


        if (request == null)
        {
            return BadRequest();
        }


        if (request.IsPasswordGrantType())
        {
            var result =
                await _authenticationService.AuthenticateAsync(
                    request.Username!,
                    request.Password!,
                    cancellationToken);


            if (!result.Success)
            {
                return Forbid(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }


            var identity =
                new ClaimsIdentity(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);


            identity.AddClaim(
                OpenIddictConstants.Claims.Subject,
                result.UserId!);


            identity.AddClaim(
                OpenIddictConstants.Claims.Name,
                result.UserName!);


            foreach (var role in result.Roles)
            {
                identity.AddClaim(
                    OpenIddictConstants.Claims.Role,
                    role);
            }


            var principal =
                new ClaimsPrincipal(identity);


            principal.SetScopes(
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Profile);


            principal.SetResources(
                "LogixSys.Api");


            return SignIn(
                principal,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }


        return BadRequest();
    }
}