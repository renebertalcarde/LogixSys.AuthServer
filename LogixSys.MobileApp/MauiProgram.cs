using Microsoft.Extensions.Logging;
using LogixSys.MobileApp.Services;

using Microsoft.Extensions.DependencyInjection;

namespace LogixSys.MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register authentication service
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            return builder.Build();
        }
    }
}
