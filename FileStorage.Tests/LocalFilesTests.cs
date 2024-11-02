using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Local;


namespace Staticsoft.FileStorage.Tests;

public class LocalFilesTests : FilesTests
{
    protected override IServiceCollection Services => base.Services
        .UseLocalFiles(_ => new LocalFilesOptions()
        {
            BasePath = Directory.CreateDirectory("FileStorage").FullName
        });
}
