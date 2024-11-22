using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Server;

public static class FileServerEndpoints
{
    const int OneGygabite = 1024 * 1024 * 1024;
    const long MaxFileSize = OneGygabite;

    public static IEndpointRouteBuilder UseFileServer(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/FileServer/{*path}", (
            HttpContext context,
            HttpRequest request,
            Files files,
            string path
        ) =>
        {
            context.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = MaxFileSize;

            return files.Write(request.Body, path);
        });

        builder.MapGet("/FileServer/{*path}", async (Files files, string path, HttpResponse response) =>
        {
            using var stream = await files.Read(path);

            await stream.CopyToAsync(response.Body);
        });
        return builder;
    }
}
