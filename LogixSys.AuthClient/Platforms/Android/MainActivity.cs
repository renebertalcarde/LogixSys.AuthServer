using Android.App;
using Android.Content.PM;
using Android.Content;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace LogixSys.AuthClient;

[Activity(
    Label = "LogixSys.AuthClient",
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    Exported = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density
)]
[IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "io.identitymodel.native", DataHost = "callback")]
public class MainActivity : MauiAppCompatActivity
{
}
