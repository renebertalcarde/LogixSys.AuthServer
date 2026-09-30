using LogixSys.AuthServer.Api.Helpers;
using LogixSys.AuthServer.Application.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace LogixSys.AuthServer.Api.Controllers;

[Route("connect")]
public class AuthorizationController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly IClaimsPrincipalFactory _claimsFactory;

    public AuthorizationController(
     IUserProfileService userProfileService,
     IClaimsPrincipalFactory claimsFactory)
    {
        _userProfileService = userProfileService;
        _claimsFactory = claimsFactory;
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize(
    CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is null)
            return BadRequest();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Challenge(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var profile = await _userProfileService.GetByIdAsync(userId, cancellationToken);

        if (profile is null || profile.Disabled)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Challenge(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var principal = _claimsFactory.Create(profile);
        principal.SetScopes(request.GetScopes());
        principal.SetResources("LogixSys.Api");

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(ClaimsPrincipalExtensions.GetDestinations(claim));
        }

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange(
    CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "The OpenID Connect request cannot be retrieved.");

        if (!request.IsAuthorizationCodeGrantType() &&
            !request.IsRefreshTokenGrantType())
        {
            return Forbid(
                new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.UnsupportedGrantType
                    }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var result = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal is null)
        {
            return Forbid(
                new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant
                    }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var subject = result.Principal.GetClaim(
            OpenIddictConstants.Claims.Subject);

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Forbid(
                new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant
                    }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var profile = await _userProfileService.GetByIdAsync(
            subject,
            cancellationToken);

        if (profile is null || profile.Disabled)
        {
            return Forbid(
                new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant
                    }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var principal = _claimsFactory.Create(profile);

        principal.SetScopes(result.Principal.GetScopes());
        principal.SetResources("LogixSys.Api");

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(
                ClaimsPrincipalExtensions.GetDestinations(claim));
        }

        return SignIn(
            principal,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}