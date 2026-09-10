using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Storage;

namespace LogixSys.MobileApp.Services;

public interface IAuthenticationService
{
    Task<bool> IsLoggedInAsync();
    Task LoginAsync();
    Task LogoutAsync();
    Task<IDictionary<string, string>?> GetProfileAsync();
}

public class AuthenticationService : IAuthenticationService
{
    private const string Authority = "https://10.0.2.2:7128"; // AuthServer.Api (development)
    private const string ClientId = "maui-client";
    private const string RedirectUri = "io.identitymodel.native://callback";
    private const string PostLogoutRedirect = "io.identitymodel.native://signout-callback";
    private readonly HttpClient _http = new();

    private const string AccessTokenKey = "access_token";
    private const string IdTokenKey = "id_token";
    private const string RefreshTokenKey = "refresh_token";

    public async Task<bool> IsLoggedInAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(AccessTokenKey);
            return !string.IsNullOrEmpty(token);
        }
        catch
        {
            return false;
        }
    }

    public async Task LoginAsync()
    {
        // PKCE values
        var codeVerifier = CreateCodeVerifier();
        var codeChallenge = CreateCodeChallenge(codeVerifier);

        var authorizeUrl = new Uri($"{Authority}/connect/authorize?client_id={ClientId}&redirect_uri={Uri.EscapeDataString(RedirectUri)}&response_type=code&scope={Uri.EscapeDataString("openid profile email api offline_access")}&code_challenge={codeChallenge}&code_challenge_method=S256");

        var callbackUrl = new Uri(RedirectUri);

        var result = await WebAuthenticator.Default.AuthenticateAsync(authorizeUrl, callbackUrl);

        if (result == null || !result.Properties.TryGetValue("code", out var code) || string.IsNullOrEmpty(code))
            throw new InvalidOperationException("Authorization did not return a code.");

        // Exchange code for tokens
        var tokenEndpoint = $"{Authority}/connect/token";
        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = RedirectUri,
            ["client_id"] = ClientId,
            ["code_verifier"] = codeVerifier
        };

        var response = await _http.PostAsync(tokenEndpoint, new FormUrlEncodedContent(body));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        if (root.TryGetProperty("access_token", out var at))
            await SecureStorage.Default.SetAsync(AccessTokenKey, at.GetString() ?? string.Empty);

        if (root.TryGetProperty("id_token", out var idt))
            await SecureStorage.Default.SetAsync(IdTokenKey, idt.GetString() ?? string.Empty);

        if (root.TryGetProperty("refresh_token", out var rt))
            await SecureStorage.Default.SetAsync(RefreshTokenKey, rt.GetString() ?? string.Empty);
    }

    public async Task LogoutAsync()
    {
        // Clear stored tokens
        try
        {
            SecureStorage.Default.Remove(AccessTokenKey);
            SecureStorage.Default.Remove(IdTokenKey);
            SecureStorage.Default.Remove(RefreshTokenKey);
        }
        catch { }

        // Optionally trigger end session at the identity provider
        var endSession = new Uri($"{Authority}/connect/endsession?post_logout_redirect_uri={Uri.EscapeDataString(PostLogoutRedirect)}&id_token_hint=");
        try
        {
            await WebAuthenticator.Default.AuthenticateAsync(endSession, new Uri(PostLogoutRedirect));
        }
        catch { }
    }

    public async Task<IDictionary<string, string>?> GetProfileAsync()
    {
        var idToken = await SecureStorage.Default.GetAsync(IdTokenKey);
        if (string.IsNullOrEmpty(idToken))
            return null;

        // Decode id_token (JWT) to extract claims without validation (for display purposes only)
        var parts = idToken.Split('.');
        if (parts.Length < 2)
            return null;

        static string Base64UrlDecode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');
            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
            }
            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        var payloadJson = Base64UrlDecode(parts[1]);
        using var doc = JsonDocument.Parse(payloadJson);
        var claims = new Dictionary<string, string?>();
        foreach (var prop in doc.RootElement.EnumerateObject())
            claims[prop.Name] = prop.Value.ToString();

        // return as non-nullable dictionary
        return claims.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value!);
    }

    private static string CreateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string CreateCodeChallenge(string codeVerifier)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
