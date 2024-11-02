using Amazon;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.FileStorage.S3;

namespace Staticsoft.FileStorage.Tests;

public class S3FilesTests : FilesTests
{
    protected override IServiceCollection Services => base.Services
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
