using IdentityModel.OidcClient.Browser;
using Microsoft.Maui.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LogixSys.AuthClient.Services;

public class WebAuthenticatorBrowser : IdentityModel.OidcClient.Browser.IBrowser
{
    readonly string _redirectUri;

    public WebAuthenticatorBrowser(string redirectUri)
    {
        _redirectUri = redirectUri;
    }

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var startUrl = options.StartUrl;
            var endUrl = _redirectUri;

            var webResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(startUrl), new Uri(endUrl));

            // WebAuthenticator returns a WebAuthenticatorResult; use its Properties dictionary
            var properties = webResult.Properties ?? new Dictionary<string, string>();

            // Construct a URL containing the returned parameters so IdentityModel can parse them
            var resultUrl = endUrl + "#" + string.Join("&", ToQueryParams(properties));

            return new BrowserResult { Response = resultUrl, ResultType = BrowserResultType.Success };
        }
        catch (TaskCanceledException)
        {
            return new BrowserResult { ResultType = BrowserResultType.UserCancel };
        }
        catch (Exception ex)
        {
            return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
        }
    }

    static IEnumerable<string> ToQueryParams(IDictionary<string, string> dict)
    {
        foreach (var kv in dict)
        {
            yield return Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value);
        }
    }
}
