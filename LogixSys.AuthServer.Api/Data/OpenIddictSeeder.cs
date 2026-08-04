using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace LogixSys.AuthServer.Api.Data;

public static class OpenIddictSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var manager =
            services.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("test-client") != null)
            return;

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "test-client",

            DisplayName = "Test Client",

            ClientType = OpenIddictConstants.ClientTypes.Public,

            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            RedirectUris =
            {
                new Uri("https://oauth.pstmn.io/v1/callback")
            },

            Permissions =
{
    OpenIddictConstants.Permissions.Endpoints.Authorization,
    OpenIddictConstants.Permissions.Endpoints.Token,

    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

    OpenIddictConstants.Permissions.ResponseTypes.Code,

    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId,
    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Profile,
    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Email,
    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
},

            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        });
    }
}