namespace Staticsoft.FileStorage.Abstractions;

public interface Files
{
    Task<string[]> List()
        => List(string.Empty);

    Task<string[]> List(string pathPrefix);

    Task<Stream> Read(string path);

    Task Write(Stream stream, string path);

    Task Move(string currentPath, string newPath);

    Task Delete(string path);

    Task<string> WriteLink(string path);

    Task<string> ReadLink(string path);
}