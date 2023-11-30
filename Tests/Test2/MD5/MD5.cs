namespace MD5;

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides methods that calculate the check-sum of a file system directory.
/// </summary>
public static class ChecksumCalculator
{
    /// <summary>
    /// Calculates the check-sum for a directory using a single-thread.
    /// </summary>
    /// <param name="directoryPath">The path to the directory.</param>
    /// <returns>The check-sum as a hex string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string CalculateDirectoryChecksumSingleThread(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException($"The directory(file) does not exist: {directoryPath}");
        }

        using var md5 = MD5.Create();
        return CalculateChecksumSingle(directoryPath, md5);
    }

    /// <summary>
    /// Calculates the check-sum for a directory using a multi-thread.
    /// </summary>
    /// <param name="directoryPath">The path to the directory.</param>
    /// <returns>The check-sum as a hex string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string CalculateDirectoryChecksumMultiThread(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException($"The directory(file) does not exist: {directoryPath}");
        }

        using var md5 = MD5.Create();
        return CalculateChecksumMulti(directoryPath, md5);
    }

    private static string CalculateChecksumMulti(string path, MD5 md5)
    {
        var combinedHashes = new StringBuilder();
        combinedHashes.Append(GetDirectoryNameHash(path, md5));

        string[] files = Directory.GetFiles(path);
        Array.Sort(files);

        string[] fileHashes = new string[files.Length];
        Parallel.For(0, files.Length, i =>
        {
            using var md5File = MD5.Create();
            fileHashes[i] = GetFileHash(files[i], md5File);
        });

        foreach (var fileHash in fileHashes)
        {
            combinedHashes.Append(fileHash);
        }

        string[] dirs = Directory.GetDirectories(path);
        Array.Sort(dirs);

        string[] directoryHashes = new string[dirs.Length];
        Parallel.For(0, dirs.Length, i =>
        {
            using var md5Dir = MD5.Create();
            directoryHashes[i] = CalculateChecksumMulti(dirs[i], md5Dir);
        });

        foreach (var dirHash in directoryHashes)
        {
            combinedHashes.Append(dirHash);
        }

        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(combinedHashes.ToString())))
        .Replace("-", "")
        .ToLowerInvariant();
    }

    private static string CalculateChecksumSingle(string path, MD5 md5)
    {
        StringBuilder hash = new();
        hash.Append(GetDirectoryNameHash(path, md5));

        foreach (var file in Directory.GetFiles(path))
        {
            hash.Append(GetFileHash(file, md5));
        }

        foreach (var dir in Directory.GetDirectories(path))
        {
            hash.Append(CalculateChecksumSingle(dir, md5));
        }

        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(hash.ToString())))
        .Replace("-", "")
        .ToLowerInvariant();
    }

    private static string GetFileHash(string filePath, MD5 md5)
    {
        using var stream = File.OpenRead(filePath);
        byte[] hashBytes = md5.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private static string GetDirectoryNameHash(string dirPath, MD5 md5)
    {
        byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(new DirectoryInfo(dirPath).Name));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
