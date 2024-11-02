using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Local;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseLocalFiles(
        this IServiceCollection services,
        Func<IServiceProvider, LocalFilesOptions> options
    )
        => services
            .AddSingleton<Files, LocalFiles>()
            .AddSingleton(options);
}
