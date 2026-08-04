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

    public AuthorizationController(
        IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
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
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Challenge(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    var profile = await _userProfileService.GetByIdAsync(
        userId,
        cancellationToken);

    if (profile is null || profile.Disabled)
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Challenge(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    var identity = new ClaimsIdentity(
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.AddClaim(
        new Claim(
            OpenIddictConstants.Claims.Subject,
            profile.UserId)
        .SetDestinations(
            OpenIddictConstants.Destinations.AccessToken,
            OpenIddictConstants.Destinations.IdentityToken));

        identity.AddClaim(
    new Claim(
        OpenIddictConstants.Claims.Name,
        profile.UserName)
    .SetDestinations(
        OpenIddictConstants.Destinations.AccessToken,
        OpenIddictConstants.Destinations.IdentityToken));

        if (!string.IsNullOrWhiteSpace(profile.Email))
    {
            identity.AddClaim(
    new Claim(
        OpenIddictConstants.Claims.Email,
        profile.Email)
    .SetDestinations(
        OpenIddictConstants.Destinations.AccessToken,
        OpenIddictConstants.Destinations.IdentityToken));
    }

    foreach (var role in profile.Roles)
    {
            identity.AddClaim(
        new Claim(OpenIddictConstants.Claims.Role, role)
            .SetDestinations(
                OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken));
        }

    var principal = new ClaimsPrincipal(identity);

    principal.SetScopes(request.GetScopes());

    principal.SetResources("resource_server");

    return SignIn(
        principal,
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
}
}