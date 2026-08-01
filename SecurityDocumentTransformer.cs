using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace LogixSys.AuthServer.Api.OpenApi;

public sealed class SecurityDocumentTransformer
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}