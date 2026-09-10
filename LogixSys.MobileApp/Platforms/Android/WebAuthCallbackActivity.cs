using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace LogixSys.MobileApp.Platforms.Android
{
    // Replace the DataSchemes/DataHosts/DataPaths below to match your callback URL.
    // Examples:
    // - callbackUri = "com.company.app://oauth2redirect" -> DataSchemes = new[] { "com.company.app" }, DataPaths = new[] { "/oauth2redirect" }
    // - callbackUri = "myapp://callback" -> DataSchemes = new[] { "myapp" }, DataHosts = new[] { "callback" }

    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTask, Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "io.identitymodel.native" }, // scheme from your callback URL
        DataHosts = new[] { "callback" })] // host from your callback URL (no path)
    public class WebAuthCallbackActivity : WebAuthenticatorCallbackActivity
    {
        // empty - base class handles the OAuth callback
    }
}
