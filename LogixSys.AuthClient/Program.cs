using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using LogixSys.AuthClient.Services;

namespace LogixSys.AuthClient;

public static class Program
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { });

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
