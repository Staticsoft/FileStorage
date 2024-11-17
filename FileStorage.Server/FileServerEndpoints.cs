using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Server;

public static class FileServerEndpoints
{
    public static IEndpointRouteBuilder UseFileServer(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/FileServer/{path}", (Files files, string path, HttpRequest request)
            => files.Write(request.Body, path)
        );
        builder.MapGet("/FileServer/{path}", async (Files files, string path, HttpResponse response) =>
        {
            using var stream = await files.Read(path);
            await stream.CopyToAsync(response.Body);
        });
        return builder;
    }
}
