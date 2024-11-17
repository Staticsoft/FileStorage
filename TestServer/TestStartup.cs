using Staticsoft.FileStorage.Server;

namespace Staticsoft.TestServer;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services) => services
        .AddSingleton(services);

    public void Configure(IApplicationBuilder app, IWebHostEnvironment _) => app
        .UseRouting()
        .UseEndpoints(endpoints => endpoints.UseFileServer());
}