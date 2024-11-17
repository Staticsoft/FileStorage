﻿using Staticsoft.FileStorage.Abstractions;

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

    public async Task Write(Stream stream, string path)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        Files[path] = memoryStream.ToArray();
    }

    public Task Delete(string path)
    {
        if (!Files.Remove(path)) throw new FileNotFoundException(path);
        return Task.CompletedTask;
    }

    public Task<string> WriteLink(string path)
        => Task.FromResult($"/FileServer/{path}");

    public Task<string> ReadLink(string path)
        => Task.FromResult($"/FileServer/{path}");
}
