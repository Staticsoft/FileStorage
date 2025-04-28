using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Memory;

namespace Staticsoft.FileStorage.Tests;

public class MemoryClientFilesTests : ClientFilesTests
{
    protected override IServiceCollection Services => base.Services
        .UseMemoryFiles();
}

public class MemoryServerFilesTests : ServerFilesTests
{
    protected override IServiceCollection ServerServices(IServiceCollection services)
        => base.ServerServices(services)
            .UseMemoryFiles();
}