using Amazon;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.S3;

namespace Staticsoft.FileStorage.Tests;

public class S3ClientFilesTests : ClientFilesTests
{
    protected override IServiceCollection Services
        => base.Services
            .UseS3();
}

public class S3ServerFilesTests : ServerFilesTests
{
    protected override IServiceCollection ServerServices(IServiceCollection services)
        => base.ServerServices(services)
            .UseS3();

    protected override IServiceCollection ClientServices(IServiceCollection services)
        => base.ClientServices(services)
            .AddSingleton<HttpClient>();
}

public static class S3Services
{
    public static IServiceCollection UseS3(this IServiceCollection services) => services
        .UseS3Files(
            _ => new AmazonS3Client(GetAccessKeyId(), GetSecretAccessKey(), GetRegion()),
            _ => new S3FilesOptions() { BucketName = GetBucketName() }
        );

    static string GetBucketName()
        => EnvVariable("FilesBucketName")!;

    static string GetAccessKeyId()
        => EnvVariable("FilesAccessKeyId")!;

    static string GetSecretAccessKey()
        => EnvVariable("FilesSecretAccessKey")!;

    static RegionEndpoint GetRegion()
        => RegionEndpoint.GetBySystemName(EnvVariable("FilesRegion"));

    static string EnvVariable(string name)
        => Environment.GetEnvironmentVariable(name)
        ?? throw new ArgumentNullException($"Environment variable {name} is not set");
}