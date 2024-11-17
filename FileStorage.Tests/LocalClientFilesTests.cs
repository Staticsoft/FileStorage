using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Local;


namespace Staticsoft.FileStorage.Tests;

public class LocalClientFilesTests : ClientFilesTests
{
    protected override IServiceCollection Services
        => base.Services
            .UseLocal();
}

public class LocalServerFilesTests : ServerFilesTests
{
    protected override IServiceCollection ServerServices(IServiceCollection services)
        => base.ServerServices(services)
            .UseLocal();
}

public static class LocalFilesServices
{
    public static IServiceCollection UseLocal(this IServiceCollection services)
        => services.UseLocalFiles(_ => new LocalFilesOptions()
        {
            BasePath = Directory.CreateDirectory("FileStorage").FullName
        });
}
