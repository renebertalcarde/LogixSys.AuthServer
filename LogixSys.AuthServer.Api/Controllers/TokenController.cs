using LogixSys.AuthServer.Api.Helpers;
using Auth = LogixSys.AuthServer.Application.Authentication;
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
    private readonly Auth.IAuthenticationService _authenticationService;
    private readonly Auth.IUserProfileService _userProfileService;
    private readonly Auth.IClaimsPrincipalFactory _claimsPrincipalFactory;

    public TokenController(
        Auth.IAuthenticationService authenticationService,
        Auth.IUserProfileService userProfileService,
        Auth.IClaimsPrincipalFactory claimsPrincipalFactory)
    {
        _authenticationService = authenticationService;
        _userProfileService = userProfileService;
        _claimsPrincipalFactory = claimsPrincipalFactory;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange(
        CancellationToken cancellationToken)
    {
        var request =
            HttpContext.GetOpenIddictServerRequest();

        if (request is null)
        {
            return BadRequest(new
            {
                error = OpenIddictConstants.Errors.InvalidRequest,
                error_description =
                    "The OpenID Connect request cannot be retrieved."
            });
        }

        Auth.UserProfile? profile;
        // ---------------------------------------------------------
        // PASSWORD GRANT
        // ---------------------------------------------------------

        if (request.IsPasswordGrantType())
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    error = OpenIddictConstants.Errors.InvalidGrant,
                    error_description =
                        "Username and password are required."
                });
            }

            var result =
                await _authenticationService.AuthenticateAsync(
                    request.Username,
                    request.Password,
                    cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    error = OpenIddictConstants.Errors.InvalidGrant,
                    error_description = result.Error
                });
            }

            profile =
                await _userProfileService.GetByIdAsync(
                    result.UserId!,
                    cancellationToken);

            if (profile is null || profile.Disabled)
            {
                return BadRequest(new
                {
                    error = OpenIddictConstants.Errors.InvalidGrant,
                    error_description =
                        "The user account is unavailable."
                });
            }

            var principal =
                _claimsPrincipalFactory.Create(profile);

            // Requested scopes
            principal.SetScopes(
                request.GetScopes());

            // API/resource
            principal.SetResources(
                "LogixSys.Api");

            // Determine which claims are allowed
            // in the access/identity tokens.
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(
                    ClaimsPrincipalExtensions
                        .GetDestinations(claim));
            }

            return SignIn(
                principal,
                OpenIddictServerAspNetCoreDefaults
                    .AuthenticationScheme);
        }

        // ---------------------------------------------------------
        // AUTHORIZATION CODE / REFRESH TOKEN
        // ---------------------------------------------------------

        if (!request.IsAuthorizationCodeGrantType() &&
            !request.IsRefreshTokenGrantType())
        {
            return BadRequest(new
            {
                error =
                    OpenIddictConstants.Errors.UnsupportedGrantType
            });
        }

        var authenticateResult =
            await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults
                    .AuthenticationScheme);

        if (!authenticateResult.Succeeded ||
            authenticateResult.Principal is null)
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults
                    .AuthenticationScheme);
        }

        var userId =
            authenticateResult.Principal.FindFirstValue(
                OpenIddictConstants.Claims.Subject);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults
                    .AuthenticationScheme);
        }

        profile =
            await _userProfileService.GetByIdAsync(
                userId,
                cancellationToken);

        if (profile is null || profile.Disabled)
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults
                    .AuthenticationScheme);
        }

        var refreshedPrincipal =
            _claimsPrincipalFactory.Create(profile);

        refreshedPrincipal.SetScopes(
            authenticateResult.Principal.GetScopes());

        refreshedPrincipal.SetResources(
            "LogixSys.Api");

        foreach (var claim in refreshedPrincipal.Claims)
        {
            claim.SetDestinations(
                ClaimsPrincipalExtensions
                    .GetDestinations(claim));
        }

        return SignIn(
            refreshedPrincipal,
            OpenIddictServerAspNetCoreDefaults
                .AuthenticationScheme);
    }
}