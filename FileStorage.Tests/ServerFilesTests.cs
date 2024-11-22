using Staticsoft.FileStorage.Abstractions;
using Staticsoft.Testing.Integration;
using Staticsoft.TestServer;
using System.Security.Cryptography;
using Xunit;

namespace Staticsoft.FileStorage.Tests;

public abstract class ServerFilesTests : IntegrationTestBase<TestStartup>
{
    const string UploadedFile = "Uploads/File";
    const string DownloadedFile = "Downloads/File";

    Files SUT
        => Server<Files>();

    HttpClient Http
        => Client<HttpClient>();

    [Fact]
    public async Task ReturnsUrlToUploadFile()
    {
        var link = await SUT.WriteLink(UploadedFile);

        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        var response = await Http.PutAsync(link, new StreamContent(writeStream));
        response.EnsureSuccessStatusCode();

        using var readStream = await SUT.Read(UploadedFile);
        using var memoryStream = new MemoryStream();
        await readStream.CopyToAsync(memoryStream);

        Assert.Equal(randomBytes, memoryStream.ToArray());
    }

    [Fact]
    public async Task ReturnsUrlToDownloadFile()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(1024);
        using var writeStream = new MemoryStream(randomBytes);
        await SUT.Write(writeStream, DownloadedFile);

        var link = await SUT.ReadLink(DownloadedFile);

        var response = await Http.GetAsync(link);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(randomBytes, data);
    }
}
