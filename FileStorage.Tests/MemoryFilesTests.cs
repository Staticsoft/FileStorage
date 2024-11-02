using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Memory;

namespace Staticsoft.FileStorage.Tests;

public class MemoryFilesTests : FilesTests
{
    protected override IServiceCollection Services => base.Services
        .UseMemoryFiles();
}
