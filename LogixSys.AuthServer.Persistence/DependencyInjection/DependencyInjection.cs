using LogixSys.AuthServer.Application.Interfaces;
using LogixSys.AuthServer.Persistence.Context;
using LogixSys.AuthServer.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogixSys.AuthServer.Persistence.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LegacyIdentityDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString(
                    "AuthDatabase"));
        });

        services.AddDbContext<OAuthDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString(
                    "OAuthDatabase"));
            options.UseOpenIddict();
        });

        services.AddOpenIddict()
            .AddCore(options => 
            {
                options.UseEntityFrameworkCore()
                .UseDbContext<OAuthDbContext>();
            });

        services.AddScoped<
            ILegacyAuthenticationRepository,
            LegacyAuthenticationRepository>();


        return services;
    }
}