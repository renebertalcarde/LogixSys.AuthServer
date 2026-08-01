using Scalar.AspNetCore;

namespace LogixSys.AuthServer.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(
        this WebApplication app)
    {
        app.UseExceptionHandler();

        app.MapOpenApi();

        app.MapScalarApiReference();

        return app;
    }
}