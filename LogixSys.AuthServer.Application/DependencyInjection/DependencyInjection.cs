using LogixSys.AuthServer.Application.Authentication;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LogixSys.AuthServer.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
            typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IClaimsPrincipalFactory, ClaimsPrincipalFactory>();

        return services;
    }
}