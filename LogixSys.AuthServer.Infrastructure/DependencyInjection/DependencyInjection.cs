using LogixSys.AuthServer.Application.Interfaces;
using LogixSys.AuthServer.Infrastructure.Authentication;
using LogixSys.AuthServer.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace LogixSys.AuthServer.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<OAuthDbContext>();
            })
            .AddServer(options =>
            {
                options.SetIssuer(
                    new Uri("https://localhost:7128/"));

                options.SetAuthorizationEndpointUris(
                    "/connect/authorize");

                options.SetTokenEndpointUris(
                    "/connect/token");

                options.AllowAuthorizationCodeFlow()
                       .RequireProofKeyForCodeExchange()
                       .AllowRefreshTokenFlow();

                options.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Roles,
                    "api");

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate()
                       .DisableAccessTokenEncryption();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });


        services.AddScoped<
            ILegacyPasswordHasher,
            LegacyPasswordHasher>();

        return services;
    }
}