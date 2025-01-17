﻿using Amazon.S3;
using Amazon.S3.Model;
using Staticsoft.FileStorage.Abstractions;
using System.Net;

namespace Staticsoft.FileStorage.S3;

public class S3FilesOptions
{
    public required string BucketName { get; init; }
}

public class S3Files(
    AmazonS3Client s3,
    S3FilesOptions options
) : Files
{
    readonly AmazonS3Client S3 = s3;
    readonly S3FilesOptions Options = options;

    public async Task<string[]> List()
    {
        var response = await S3.ListObjectsAsync(Options.BucketName);
        return response.S3Objects.Select(s3Object => s3Object.Key).ToArray();
    }

    public async Task<Stream> Read(string path)
    {
        try
        {
            var response = await S3.GetObjectAsync(new() { BucketName = Options.BucketName, Key = path });
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException(path);
        }
    }

    public async Task Write(Stream stream, string path)
    {
        await S3.PutObjectAsync(new() { BucketName = Options.BucketName, Key = path, InputStream = stream });
    }

    public async Task Move(string currentPath, string newPath)
    {
        try
        {
            await S3.GetObjectMetadataAsync(Options.BucketName, currentPath);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException(currentPath);
        }

        await S3.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = Options.BucketName,
            SourceKey = currentPath,
            DestinationBucket = Options.BucketName,
            DestinationKey = newPath
        });

        await Delete(currentPath);
    }

    public async Task Delete(string path)
    {
        try
        {
            await S3.GetObjectMetadataAsync(Options.BucketName, path);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException(path);
        }

        await S3.DeleteObjectAsync(new() { BucketName = Options.BucketName, Key = path });
    }

    public Task<string> WriteLink(string path)
        => S3.GetPreSignedURLAsync(new()
        {
            BucketName = Options.BucketName,
            Key = path,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(1)
        });

    public Task<string> ReadLink(string path)
        => S3.GetPreSignedURLAsync(new()
        {
            BucketName = Options.BucketName,
            Key = path,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(1)
        });
}
