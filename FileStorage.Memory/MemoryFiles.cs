using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Memory;

public class MemoryFiles : Files
{
    readonly Dictionary<string, byte[]> Files = [];

    public Task<string[]> List()
        => Task.FromResult(Files.Keys.ToArray());

    public Task<Stream> Read(string path)
        => Task.FromResult((Stream)new MemoryStream(ReadFile(path)));

    byte[] ReadFile(string path)
        => Files.TryGetValue(path, out var bytes)
        ? bytes
        : throw new FileNotFoundException(path);

    public Task Write(Stream stream, string path)
    {
        using var memoryStream = new MemoryStream();
        {
            stream.CopyTo(memoryStream);
            Files[path] = memoryStream.ToArray();
        }
        return Task.CompletedTask;
    }

    public Task Delete(string path)
    {
        if (!Files.Remove(path)) throw new FileNotFoundException(path);
        return Task.CompletedTask;
    }
}
