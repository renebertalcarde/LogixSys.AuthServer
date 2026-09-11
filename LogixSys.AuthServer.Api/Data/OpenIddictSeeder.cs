using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

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

        application = await manager.FindByClientIdAsync("maui-client");

        if (application is null)
        {
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "maui-client",
                    DisplayName = "LogixSys MAUI Client",
                    ClientType = ClientTypes.Public, // native/public client — no client secret
                    RedirectUris =
                    {
                        new Uri("io.identitymodel.native://callback"), // must match registered redirect for native apps
                        // Add loopback redirect for Windows (system browser + loopback) so MAUI on Windows can use Authorization Code + PKCE
                        new Uri("http://127.0.0.1:7890/callback")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("io.identitymodel.native://signout-callback"),
                        new Uri("http://127.0.0.1:7890/signout-callback")
                    },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // enforce PKCE
                    }
                });
        }
        else
        {
            // If the application already exists, ensure it has the loopback redirect URIs and post-logout URIs.
            var updateDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = await manager.GetClientIdAsync(application),
                ClientType = await manager.GetClientTypeAsync(application),
                // then set RedirectUris / PostLogoutRedirectUris etc.
            };

            var redirects = new List<Uri>();

            // Use manager to read redirect URIs from the opaque application object
            var existingRedirects = await manager.GetRedirectUrisAsync(application);
            if (existingRedirects != null)
            {
                foreach (var u in existingRedirects)
                    redirects.Add(new Uri(u));
            }

            // ensure native scheme exists
            if (!redirects.Exists(u => u.AbsoluteUri == "io.identitymodel.native://callback"))
                redirects.Add(new Uri("io.identitymodel.native://callback"));

            // ensure loopback exists
            if (!redirects.Exists(u => u.AbsoluteUri == "http://127.0.0.1:7890/callback"))
                redirects.Add(new Uri("http://127.0.0.1:7890/callback"));

            foreach (var uri in redirects)
                updateDescriptor.RedirectUris.Add(uri);

            var postLogout = new List<Uri>();

            // Use manager to read post-logout URIs
            var existingPostLogout = await manager.GetPostLogoutRedirectUrisAsync(application);
            if (existingPostLogout != null)
            {
                foreach (var u in existingPostLogout)
                    postLogout.Add(new Uri(u));
            }

            if (!postLogout.Exists(u => u.AbsoluteUri == "io.identitymodel.native://signout-callback"))
                postLogout.Add(new Uri("io.identitymodel.native://signout-callback"));

            if (!postLogout.Exists(u => u.AbsoluteUri == "http://127.0.0.1:7890/signout-callback"))
                postLogout.Add(new Uri("http://127.0.0.1:7890/signout-callback"));

            foreach (var uri in postLogout)
                updateDescriptor.PostLogoutRedirectUris.Add(uri);

            // Persist the update
            await manager.UpdateAsync(application, updateDescriptor);
        }
    }

}