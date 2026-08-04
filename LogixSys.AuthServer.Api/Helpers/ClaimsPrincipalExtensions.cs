using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace LogixSys.AuthServer.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> GetDestinations(
        this Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.Subject:
            case ClaimTypes.Name:
            case ClaimTypes.Role:

                return new[]
                {
                    OpenIddictConstants.Destinations.AccessToken,
                    OpenIddictConstants.Destinations.IdentityToken
                };

            default:

                return new[]
                {
                    OpenIddictConstants.Destinations.AccessToken
                };
        }
    }
}