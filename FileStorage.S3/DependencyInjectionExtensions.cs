using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.S3;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseS3Files(
        this IServiceCollection services,
        Func<IServiceProvider, AmazonS3Client> s3,
        Func<IServiceProvider, TransferUtility> transfer,
        Func<IServiceProvider, S3FilesOptions> options
    ) => services
        .AddSingleton<Files, S3Files>()
        .AddSingleton(s3)
        .AddSingleton(transfer)
        .AddSingleton(options);
}
