﻿using System.Text.Json;
using System.Text.RegularExpressions;
using Fantastic.FileSystem;

namespace ImportBuddy;

public static class FileSystemExtensions
{
    public static async Task<T?> Deserialize<T>(this IFileSystem fileSystem, string path, CancellationToken cancellationToken = default)
    {
        if (!await fileSystem.File.Exists(path))
        {
            return default;
        }

        string json = await fileSystem.File.ReadAllText(path, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, JsonHelper.JsonOptions);
    }

    public static string CleanPath(this IFileSystem fileSystem, string name)
    {
        string invalidChars = Regex.Escape(new string(fileSystem.Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return Regex.Replace(name, invalidRegStr, "")
            .Replace('·', ' '); // makemkv doesn't like this char
    }

    public static async Task Download(this HttpClient httpClient, IFileSystem fileSystem, string url, string path)
    {
        var bytes = await httpClient.GetByteArrayAsync(url);
        await fileSystem.File.WriteAllBytes(path, bytes);
    }
}
