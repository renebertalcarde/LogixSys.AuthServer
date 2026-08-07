using LogixSys.AuthServer.Application.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace LogixSys.AuthServer.Api.Controllers;

[Route("connect")]
public class TokenController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly IClaimsPrincipalFactory _claimsPrincipalFactory;

    public TokenController(
        IUserProfileService userProfileService,
        IClaimsPrincipalFactory claimsPrincipalFactory)
    {
        _userProfileService = userProfileService;
        _claimsPrincipalFactory = claimsPrincipalFactory;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange(
        CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is null)
        {
            return BadRequest();
        }

        if (!request.IsAuthorizationCodeGrantType() &&
            !request.IsRefreshTokenGrantType())
        {
            return BadRequest(new
            {
                error = OpenIddictConstants.Errors.UnsupportedGrantType
            });
        }

        var authenticateResult =
            await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded ||
            authenticateResult.Principal is null)
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var userId = authenticateResult.Principal.FindFirstValue(
            OpenIddictConstants.Claims.Subject);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var profile = await _userProfileService.GetByIdAsync(
            userId,
            cancellationToken);

        if (profile is null || profile.Disabled)
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        //
        // Rebuild a fresh ClaimsPrincipal from the latest database state.
        //
        var principal = _claimsPrincipalFactory.Create(profile);

        return SignIn(
            principal,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}