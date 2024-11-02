using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Memory;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseMemoryFiles(this IServiceCollection services) => services
        .AddSingleton<Files, MemoryFiles>();
}
