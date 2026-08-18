using OpenIddict.Abstractions;

namespace LogixSys.AuthServer.Api.Data;

public static class OpenIddictSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        var application = await manager.FindByClientIdAsync("test-client");

        if (application is null)
        {
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "test-client",

                    DisplayName = "Test Client",

                    ClientType =
                        OpenIddictConstants.ClientTypes.Public,

                    ConsentType =
                        OpenIddictConstants.ConsentTypes.Explicit,

                    RedirectUris =
                    {
                    new Uri("https://localhost:7128/signin-oidc")
                    },

                    Permissions =
                    {
                    // Endpoints
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    // Grant types
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    // Response type
                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    // Scopes
                    OpenIddictConstants.Permissions.Prefixes.Scope
                        + OpenIddictConstants.Scopes.OpenId,

                    OpenIddictConstants.Permissions.Prefixes.Scope
                        + OpenIddictConstants.Scopes.Profile,

                    OpenIddictConstants.Permissions.Prefixes.Scope
                        + OpenIddictConstants.Scopes.Email,

                    OpenIddictConstants.Permissions.Prefixes.Scope
                        + "api"
                    },

                    Requirements =
                    {
                    OpenIddictConstants.Requirements.Features
                        .ProofKeyForCodeExchange
                    }
                });
        }

        application = await manager.FindByClientIdAsync("doar-web");

        if (application is null)
        {
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "doar-web",
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    RedirectUris =
                    {
                        new Uri("http://localhost:3000/auth/callback")
                    },

                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess
                    },
                    Requirements =
                    {
                    OpenIddictConstants.Requirements.Features
                        .ProofKeyForCodeExchange
                    }
                });
        }
    }

}