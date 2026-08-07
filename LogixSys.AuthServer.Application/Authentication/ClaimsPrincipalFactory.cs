using System.Security.Claims;
using OpenIddict.Abstractions;

namespace LogixSys.AuthServer.Application.Authentication;

public sealed class ClaimsPrincipalFactory : IClaimsPrincipalFactory
{
    public ClaimsPrincipal Create(UserProfile profile)
    {
        var identity = new ClaimsIdentity("LogixSys");

        //
        // Subject (required by OpenID Connect)
        //
        identity.AddClaim(new Claim(
            OpenIddictConstants.Claims.Subject,
            profile.UserId));

        //
        // ASP.NET compatibility
        //
        identity.AddClaim(new Claim(
            ClaimTypes.NameIdentifier,
            profile.UserId));

        //
        // Username
        //
        identity.AddClaim(new Claim(
            OpenIddictConstants.Claims.Name,
            profile.UserName));

        identity.AddClaim(new Claim(
            ClaimTypes.Name,
            profile.UserName));

        //
        // Email
        //
        if (!string.IsNullOrWhiteSpace(profile.Email))
        {
            identity.AddClaim(new Claim(
                OpenIddictConstants.Claims.Email,
                profile.Email));

            identity.AddClaim(new Claim(
                ClaimTypes.Email,
                profile.Email));
        }

        //
        // Roles
        //
        foreach (var role in profile.Roles)
        {
            identity.AddClaim(new Claim(
                OpenIddictConstants.Claims.Role,
                role));

            identity.AddClaim(new Claim(
                ClaimTypes.Role,
                role));
        }

        return new ClaimsPrincipal(identity);
    }
}