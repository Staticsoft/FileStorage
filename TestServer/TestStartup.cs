using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.TestServer;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services) => services
        .AddSingleton(services);

    public void Configure(IApplicationBuilder app, IWebHostEnvironment _) => app
        .UseRouting()
        .UseEndpoints(UseFilesServer);

    static void UseFilesServer(IEndpointRouteBuilder builder)
    {
        builder.MapPut("/FileServer/{path}", (Files files, string path, HttpRequest request)
            => files.Write(request.Body, path)
        );
        builder.MapGet("/FileServer/{path}", async (Files files, string path, HttpResponse response) =>
        {
            using var stream = await files.Read(path);
            await stream.CopyToAsync(response.Body);
        });
    }
}