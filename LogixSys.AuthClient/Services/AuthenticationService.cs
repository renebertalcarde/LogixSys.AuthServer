using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Maui.Authentication;
using System;
using System.Threading.Tasks;

namespace LogixSys.AuthClient.Services;

public class AuthenticationService : IAuthenticationService
{
    readonly OidcClient _oidcClient;

    // TODO: update these values to match your AuthServer configuration
    const string Authority = "https://localhost:5081"; // your AuthServer base URL
    const string ClientId = "maui-client"; // register this client on the AuthServer
    const string RedirectUri = "io.identitymodel.native://callback"; // must match registered redirect

    public AuthenticationService()
    {
        var options = new OidcClientOptions
        {
            Authority = Authority,
            ClientId = ClientId,
            Scope = "openid profile api offline_access",
            RedirectUri = RedirectUri,
            Browser = (IdentityModel.OidcClient.Browser.IBrowser)new WebAuthenticatorBrowser(RedirectUri),
            Policy = new Policy { RequireAccessTokenHash = false }
        };

        _oidcClient = new OidcClient(options);
    }

    public async Task<AuthResult> LoginAsync()
    {
        try
        {
            var result = await _oidcClient.LoginAsync(new LoginRequest());
            if (result.IsError)
                return new AuthResult(true, result.Error, null, null);

            var name = result.User?.FindFirst(c => c.Type == "name")?.Value ?? result.User?.Identity?.Name;
            return new AuthResult(false, null, name, result.AccessToken);
        }
        catch (Exception ex)
        {
            return new AuthResult(true, ex.Message, null, null);
        }
    }

    public Task LogoutAsync()
    {
        // For native apps you generally just clear local tokens. For server logout implement if needed.
        return Task.CompletedTask;
    }
}
