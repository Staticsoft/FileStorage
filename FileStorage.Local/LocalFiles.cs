using Staticsoft.FileStorage.Abstractions;

namespace Staticsoft.FileStorage.Local;

public class LocalFilesOptions
{
    public required string BasePath { get; init; }
}

public class LocalFiles(
    LocalFilesOptions options
) : Files
{
    readonly LocalFilesOptions Options = options;
    readonly int RelativeIndex = $"{options.BasePath}{Path.DirectorySeparatorChar}".Length;

    public Task<string[]> List(string pathPrefix)
        => Task.FromResult(ListFiles(pathPrefix));

    string[] ListFiles(string pathPrefix)
        => Directory
            .GetFiles(Options.BasePath, "*", SearchOption.AllDirectories)
            .Select(ToRelativePath)
            .Select(ToConsistentPathSeparator)
            .Where(file => file.StartsWith(pathPrefix))
            .ToArray();

    string ToRelativePath(string filePath)
        => filePath[RelativeIndex..];

    string ToConsistentPathSeparator(string filePath)
        => filePath.Replace('\\', '/');

    public Task<Stream> Read(string path)
        => Task.FromResult((Stream)File.OpenRead(ToFilePath(path)));

    public async Task Write(Stream stream, string path)
    {
        var filePath = ToFilePath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using var writeStream = File.OpenWrite(filePath);
        await stream.CopyToAsync(writeStream);
    }

    public Task Move(string currentPath, string newPath)
    {
        var currentFilePath = ToFilePath(currentPath);
        var newFilePath = ToFilePath(newPath);

        if (!File.Exists(currentFilePath)) throw new FileNotFoundException(currentPath);

        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath)!);

        File.Move(currentFilePath, newFilePath, overwrite: true);

        return Task.CompletedTask;
    }

    public Task Delete(string path)
    {
        var filePath = ToFilePath(path);
        if (!File.Exists(filePath)) throw new FileNotFoundException(path);

        File.Delete(filePath);
        return Task.CompletedTask;
    }

    string ToFilePath(string path)
        => Path.Combine(Options.BasePath, path);

    public Task<string> WriteLink(string path)
        => Task.FromResult($"/FileServer/{path}");

    public Task<string> ReadLink(string path)
        => Task.FromResult($"/FileServer/{path}");
}
