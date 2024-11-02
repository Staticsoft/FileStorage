﻿namespace Staticsoft.FileStorage.Abstractions;

public interface Files
{
    Task<string[]> List();

    Task<Stream> Read(string path);

    Task Write(Stream stream, string path);

    Task Delete(string path);
}