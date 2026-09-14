using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HRMobile;

public partial class MainPage : ContentPage
{
    private readonly OidcClient _oidcClient;
    private readonly TokenManager _tokenManager;
    private readonly HttpClient _http;

    public MainPage(IConfiguration configuration)
    {
        InitializeComponent();

        // read settings
        var oidcSection = configuration.GetSection("Oidc");
        var authority = oidcSection["Authority"]!;
        var clientId = oidcSection["ClientId"]!;
        var scope = oidcSection["Scope"] ?? "openid profile offline_access";
        var loopbackPort = int.Parse(oidcSection["LoopbackPort"] ?? "7890");
        var redirectUri = $"http://127.0.0.1:{loopbackPort}/";

        // create OidcClient with parameterized LoopbackBrowser
        var browser = new LoopbackBrowser(loopbackPort);
        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            RedirectUri = redirectUri,
            Scope = scope,
            Browser = browser
        };
        _oidcClient = new OidcClient(options);
        _tokenManager = new TokenManager(_oidcClient);

        _http = new HttpClient(new RefreshOnUnauthorizedHandler(_tokenManager))
        {
            BaseAddress = new Uri(authority) // adjust to your API base
        };

        SignInButton.Clicked += SignInButton_Clicked;
        RefreshButton.Clicked += RefreshButton_Clicked;
        CallApiButton.Clicked += CallApiButton_Clicked;
    }

    private async void SignInButton_Clicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Signing in...";
        var result = await _tokenManager.LoginAsync();
        if (result.IsError)
        {
            StatusLabel.Text = $"Sign-in error: {result.Error}";
            return;
        }

        StatusLabel.Text = $"Signed in. Access token length: {result.AccessToken?.Length ?? 0}";
    }

    private async void RefreshButton_Clicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Refreshing token...";
        var ok = await _tokenManager.TryRefreshAsync();
        StatusLabel.Text = ok ? "Refresh succeeded." : "Refresh failed or no refresh token.";
    }

    private async void CallApiButton_Clicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Calling API...";
        try
        {
            var resp = await _http.GetAsync("/api/protected");
            var body = await resp.Content.ReadAsStringAsync();
            StatusLabel.Text = $"Status: {resp.StatusCode}\n{body}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"HTTP error: {ex.Message}";
        }
    }

    // -----------------------
    // Helper classes
    // -----------------------

    public class LoopbackBrowser : Duende.IdentityModel.OidcClient.Browser.IBrowser
    {
        private readonly int _port;
        private readonly string _ip;

        public LoopbackBrowser(int port = 7890, string ip = "127.0.0.1")
        {
            _port = port;
            _ip = ip;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var prefix = $"http://{_ip}:{_port}/";
            using var listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            // open system browser
            var psi = new ProcessStartInfo(options.StartUrl) { UseShellExecute = true };
            Process.Start(psi);

            var context = await listener.GetContextAsync();

            var responseString = "<html><body>Authentication complete. You may close this window.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            listener.Stop();

            var redirectUri = context.Request.Url!.ToString();
            return new BrowserResult { Response = redirectUri, ResultType = BrowserResultType.Success };
        }

        public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
        {
            var psi = new ProcessStartInfo(uri.ToString()) { UseShellExecute = true };
            Process.Start(psi);
            return Task.FromResult(true);
        }
    }

    public class TokenManager
    {
        private readonly OidcClient _oidcClient;
        private const string AccessKey = "access_token";
        private const string RefreshKey = "refresh_token";

        public TokenManager(OidcClient oidcClient) => _oidcClient = oidcClient;

        public async Task<LoginResult> LoginAsync()
        {
            var loginRequest = new LoginRequest();
            var result = await _oidcClient.LoginAsync(loginRequest);
            if (!result.IsError)
            {
                if (!string.IsNullOrEmpty(result.AccessToken))
                    await SecureStorage.Default.SetAsync(AccessKey, result.AccessToken);
                if (!string.IsNullOrEmpty(result.RefreshToken))
                    await SecureStorage.Default.SetAsync(RefreshKey, result.RefreshToken);
            }
            return result;
        }

        public async Task<bool> TryRefreshAsync()
        {
            var storedRefresh = await SecureStorage.Default.GetAsync(RefreshKey);
            if (string.IsNullOrEmpty(storedRefresh))
                return false;

            var refreshResult = await _oidcClient.RefreshTokenAsync(storedRefresh);
            if (refreshResult.IsError)
                return false;

            if (!string.IsNullOrEmpty(refreshResult.AccessToken))
                await SecureStorage.Default.SetAsync(AccessKey, refreshResult.AccessToken);
            if (!string.IsNullOrEmpty(refreshResult.RefreshToken))
                await SecureStorage.Default.SetAsync(RefreshKey, refreshResult.RefreshToken);

            return true;
        }

        public async Task<string?> GetAccessTokenAsync() => await SecureStorage.Default.GetAsync(AccessKey);
    }

    public class RefreshOnUnauthorizedHandler : DelegatingHandler
    {
        private readonly TokenManager _tokenManager;

        public RefreshOnUnauthorizedHandler(TokenManager tokenManager, HttpMessageHandler? inner = null)
        {
            _tokenManager = tokenManager;
            InnerHandler = inner ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenManager.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // Try refresh once
            var refreshed = await _tokenManager.TryRefreshAsync();
            if (!refreshed)
                return response;

            var newToken = await _tokenManager.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(newToken))
            {
                var retry = await CloneHttpRequestMessageAsync(request);
                retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response.Dispose();
                return await base.SendAsync(retry, cancellationToken);
            }

            return response;
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri) { Version = req.Version };

            if (req.Content != null)
            {
                var ms = new MemoryStream();
                await req.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
