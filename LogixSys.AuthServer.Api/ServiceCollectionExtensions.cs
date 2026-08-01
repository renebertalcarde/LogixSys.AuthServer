using Microsoft.AspNetCore.OpenApi;
using LogixSys.AuthServer.Api.OpenApi;

namespace LogixSys.AuthServer.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<SecurityDocumentTransformer>();
        });

        return services;
    }
}