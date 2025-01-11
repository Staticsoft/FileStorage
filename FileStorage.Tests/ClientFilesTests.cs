using Staticsoft.FileStorage.Abstractions;
using Staticsoft.Testing;
using System.Security.Cryptography;
using Xunit;

namespace Staticsoft.FileStorage.Tests;

public abstract class ClientFilesTests : TestBase<Files>, IAsyncLifetime
{
    const string NonExistingFile = "NonExisting";
    const string ExistingFile = "ExistingFile";
    const string NewLocation = "NewLocation";

    public async Task InitializeAsync()
    {
        var files = await SUT.List();
        await Task.WhenAll(files.Select(SUT.Delete));
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    [Fact]
    public async Task ThrowsNotFoundExceptionWhenReadingNonExistingFile()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => SUT.Read(NonExistingFile));
    }

    [Fact]
    public async Task ReturnsStreamWhenReadingExistingFile()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        await SUT.Write(writeStream, ExistingFile);

        using var readStream = await SUT.Read(ExistingFile);
        using var memoryStream = new MemoryStream();
        await readStream.CopyToAsync(memoryStream);

        Assert.Equal(randomBytes, memoryStream.ToArray());
    }

    [Fact]
    public async Task ReturnsStreamWithOverwrittenContentsAfterFileIsOverwritten()
    {
        var firstRandomBytes = RandomNumberGenerator.GetBytes(1024);
        using var firstWriteStream = new MemoryStream(firstRandomBytes);
        await SUT.Write(firstWriteStream, ExistingFile);

        var secondRandomBytes = RandomNumberGenerator.GetBytes(1024);
        using var secondWriteStream = new MemoryStream(secondRandomBytes);
        await SUT.Write(secondWriteStream, ExistingFile);

        using var readStream = await SUT.Read(ExistingFile);
        using var memoryStream = new MemoryStream();
        await readStream.CopyToAsync(memoryStream);

        Assert.Equal(secondRandomBytes, memoryStream.ToArray());
    }

    [Fact]
    public async Task ReturnsEmptyListOfFiles()
    {
        var files = await SUT.List();
        Assert.Empty(files);
    }

    [Fact]
    public async Task ReturnsNonEmptyListOfFilesAfterFileIsCreated()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        await SUT.Write(writeStream, ExistingFile);

        var files = await SUT.List();
        Assert.Equal([ExistingFile], files);
    }

    [Fact]
    public async Task ThrowsNotFoundExceptionWhenDeletingNonExistingFile()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => SUT.Delete(NonExistingFile));
    }

    [Fact]
    public async Task ReturnsEmptyListOfFilesAfterDeletingFile()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        await SUT.Write(writeStream, ExistingFile);

        await SUT.Delete(ExistingFile);

        var files = await SUT.List();
        Assert.Empty(files);
    }

    [Fact]
    public async Task ThrowsNotFoundExceptionWhenMovingNonExistingFile()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => SUT.Move(NonExistingFile, NewLocation));
    }

    [Fact]
    public async Task MovesFileFromOneLocationToAnother()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        await SUT.Write(writeStream, ExistingFile);

        await SUT.Move(ExistingFile, NewLocation);

        using var readStream = await SUT.Read(NewLocation);
        using var memoryStream = new MemoryStream();
        await readStream.CopyToAsync(memoryStream);

        Assert.Equal(randomBytes, memoryStream.ToArray());

        var files = await SUT.List();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(NewLocation, file);
    }
}
