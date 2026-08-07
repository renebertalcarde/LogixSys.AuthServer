using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace LogixSys.AuthServer.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> GetDestinations(this Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or Claims.Email or OpenIddictConstants.Claims.Subject =>
            [
                OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken
            ],
            OpenIddictConstants.Claims.Role =>
            [
                OpenIddictConstants.Destinations.AccessToken
            ],
            ClaimTypes.Name or ClaimTypes.Role =>
            [
                OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken
            ],
            _ => 
            [
                OpenIddictConstants.Destinations.AccessToken
            ],
        };
    }
}